using System.Collections.Generic;
using System.Collections.Specialized;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors.StudentAI
{
    public class Flock
    {
        public float AlignmentStrength { get; set; }
        public float CohesionStrength { get; set; }
        public float SeparationStrength { get; set; }
        public List<MovingObject> Boids { get; protected set; }
        public Vector3 AveragePosition { get; set; }
        protected Vector3 AverageForward { get; set; }
        public float FlockRadius { get; set; }

        public Flock() {
            Boids = new List<MovingObject>();
            AlignmentStrength = 1.0f;
            CohesionStrength = 1.0f;
            SeparationStrength = 1.0f;
            FlockRadius = 10.0f;
        }
        #region TODO
        public virtual void Update(float deltaTime)
        {
            AveragePosition = Vector3.Zero;
            AverageForward = Vector3.Zero;

            foreach (MovingObject boid in Boids)
            {
                AveragePosition += boid.Position;
                AverageForward += boid.Velocity;
            }

            AveragePosition /= Boids.Count;
            AverageForward /= Boids.Count;

            foreach (MovingObject boid in Boids)
            {
                Vector3 accel = CalcAlignmentAccel(boid);
                accel += CalcCohesionAccel(boid);
                accel += CalcSeparationAccel(boid);
                accel *= boid.MaxSpeed * deltaTime;
                boid.Velocity += accel;

                // Cap magnitude based on the boid's max speed
                if (boid.Velocity.Length > boid.MaxSpeed)
                {
                    Vector3.Normalize(boid.Velocity);
                    boid.Velocity *= boid.MaxSpeed;
                }

                boid.Update(deltaTime);
            }
        }
        public Vector3 CalcAlignmentAccel(MovingObject boid)
        {
            Vector3 accel = AverageForward / boid.MaxSpeed;
            if(accel.Length > 1)
            {
                accel.Normalize();
            }
            return accel * AlignmentStrength;
        }

        public Vector3 CalcCohesionAccel(MovingObject boid)
        {
            Vector3 accel = AveragePosition - boid.Position;
            float distance = accel.Length;
            accel.Normalize();
            if(distance < FlockRadius)
            {
                accel *= distance / FlockRadius;
            }
            return accel * CohesionStrength;
        }
        public Vector3 CalcSeparationAccel(MovingObject boid)
        {
            Vector3 sum = Vector3.Zero;
            foreach(MovingObject sibling in Boids)
            {
                if(boid == sibling)
                {
                    continue;
                }
                Vector3 accel = boid.Position - sibling.Position;
                float distance = accel.Length;
                float safeDistance = boid.SafeRadius + sibling.SafeRadius;
                if(distance < safeDistance)
                {
                    accel.Normalize();
                    accel *= (safeDistance - distance) / safeDistance;
                    sum += accel;
                }

            }
            if(sum.Length > 1.0f)
            {
                sum.Normalize();
            }
            return sum * SeparationStrength;
        }
        #endregion
    }
}

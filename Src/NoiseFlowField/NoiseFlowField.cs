using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using t4ccer.Noisy;
using t4ccer.SharpProcessing;

namespace Debug
{
    public class NoiseFlowField : Processing
    {
        public class Particle : Processing
        {
            public PVector pos;
            private readonly PVector prevPos;
            private readonly PVector vel;
            private const int maxSpeed = 5;

            public Particle()
            {
                pos = new PVector(Random(Width), Random(Height));
                prevPos = new PVector(pos.X, pos.Y);
                vel = new PVector(0, 0);
            }
            public void Update(PVector force)
            {
                vel.Add(force);
                vel.Limit(maxSpeed);
                pos.Add(vel);

                if (pos.X >= Width)
                {
                    pos.X = prevPos.X = 0;
                }
                else if (pos.X < 0)
                {
                    pos.X = prevPos.X = Width - 1;
                }

                if (pos.Y >= Height)
                {
                    pos.Y = prevPos.Y = 0;
                }
                else if (pos.Y < 0)
                {
                    pos.Y = prevPos.Y = Height - 1;
                }
            }
            public void Show()
            {
                Line(pos.X, pos.Y, prevPos.X, prevPos.Y);
                prevPos.X = pos.X;
                prevPos.Y = pos.Y;
            }
        }

        private readonly byte alpha = 1;
        private readonly int scale = 10;
        private readonly float force = 2;
        private readonly double xyInc = 0.02;
        private readonly double zInc = 0.05;
        private readonly int saveRate = 100;
        private readonly int particleCount = 10000;
        private readonly double angleRange = Math.PI * 2;

        private readonly string runId = Guid.NewGuid().ToString().Replace("-", "");
        private INoise noise;
        private PVector[,] vectors;
        private List<Particle> particles;

        private double z = 0;


        public NoiseFlowField(byte alpha, int particleCount, int scale, float force, double xyInc, double zInc, int saveRate, double angleRange)
        {
            this.alpha = alpha;
            this.particleCount = particleCount;
            this.scale = scale;
            this.force = force;
            this.xyInc = xyInc;
            this.zInc = zInc;
            this.saveRate = saveRate;
            this.angleRange = angleRange;
        }

        public override void Setup()
        {
            Size(1200, 800);
            Background(0);

            //Draw grid
            //Stroke(50, 0, 0);
            //for (var y = 0; y < Height; y += scale)
            //{
            //    for (var x = 0; x < Width; x += scale)
            //    {
            //        Line(x, 0, x, Height);
            //    }
            //    Line(0, y, Width, y);
            //}


            Stroke(255, alpha);
            StrokeWeight(1);

            noise = new OpenSimplexNoise3DGenerator();
            vectors = new PVector[Width / scale, Height / scale];
            particles = new List<Particle>(particleCount);
            for (var i = 0; i < particleCount; i++)
            {
                particles.Add(new Particle());
            }
        }
        public override void Draw()
        {
            var anglePlane = noise.AtPlaneParallel(0, 0, z, Width, Height, xyInc).Map(0, 1, 0, angleRange);
            var forcePlane = noise.AtPlaneParallel(0, 0, z + 1337, Width, Height, xyInc).Normalize().Map(0, 1, 0, force);

            for (var y = 0; y < Height; y += scale)
            {
                for (var x = 0; x < Width; x += scale)
                {
                    var x1 = x / scale;
                    var y1 = y / scale;

                    var v = PVector.FromAngle(anglePlane[x1, y1]);
                    vectors[x1, y1] = v;

                    v.SetMag((float)forcePlane[x, y]);
                    //v.SetMag(force);
                }
            }

            Parallel.ForEach(particles, p =>
            {
                var x = (int)p.pos.X / scale;
                var y = (int)p.pos.Y / scale;
                p.Update(vectors[x, y]);
            });

            foreach (var p in particles)
            {
                p.Show();
            }
            z += zInc;

            if (FrameCount % saveRate == 0 && saveRate > 0)
            {
                var filename = $"out{runId}_{FrameCount}.png";
                SaveFrame(filename);
                Console.WriteLine(filename);
            }
        }
    }
}

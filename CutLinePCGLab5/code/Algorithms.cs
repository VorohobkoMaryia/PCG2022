using System;
using System.Collections.Generic;
using System.Drawing;

namespace PKG_5
{
    struct Vector
    {
        public Vector(KeyValuePair<PointF, PointF> a)
        {
            A = a.Key.X - a.Value.X;
            B = a.Key.Y - a.Value.Y;
        }
        public Vector(float Vx, float Vy)
        {
            this.A = Vx;
            this.B = Vy;
        }
        public Vector(PointF begin, PointF end)
        {
            A = begin.X - end.X;
            B = begin.Y - end.Y;
        }
        public float A;
        public float B;
    }
    struct forSutherlandCohen
    {
        public forSutherlandCohen(Point min, Point max)
        {
            minPoint = min;
            pMax = max;
        }
        public forSutherlandCohen(float x_min, float y_min, float x_max, float y_max)
        {
            minPoint = new PointF(x_min, y_min);
            pMax = new PointF(x_max, y_max);
        }
        public PointF minPoint;
        public PointF pMax;
    }
    class Algorithms
    {
        public void SetRectangleClipper(float x_min, float y_min, float x_max, float y_max)
        {
            rect = new forSutherlandCohen(x_min, y_min, x_max, y_max);
        }
        public void SetRectangleClipper(Point min, Point max)
        {
            rect = new forSutherlandCohen(min, max);
        }
        
        public string getCode(PointF a)
        {
            string code = "";
            code += (a.Y > rect.pMax.Y ? '1' : '0');
            code += (a.Y < rect.minPoint.Y ? '1' : '0');
            code += (a.X > rect.pMax.X ? '1' : '0');
            code += (a.X < rect.minPoint.X ? '1' : '0');
            return code;
        }
        public PointF moveBit(PointF a, float k)
        {
            var code = getCode(a);
            if (code[2] == '1')
            {
                return new PointF(rect.pMax.X, a.Y + k * (rect.pMax.X - a.X));
            }
            if (code[3] == '1')
            {
                return new PointF(rect.minPoint.X, a.Y + k * (rect.minPoint.X - a.X));
            }
            if (code[0] == '1')
            {
                return new PointF(a.X + (1 / k) * (rect.pMax.Y - a.Y), rect.pMax.Y);
            }
            if (code[1] == '1')
            {
                return new PointF(a.X + (1 / k) * (rect.minPoint.Y - a.Y), rect.minPoint.Y);
            }
            return a;
        }
        public bool SutherlandCohen(PointF a, PointF b, ref PointF p1, ref PointF p2)
        {
            p1 = a;
            p2 = b;
            var a_code = getCode(a);
            var b_code = getCode(b);
            if (a_code.Equals("0000") && b_code.Equals("0000")) {
                p1 = new PointF(a.X, a.Y);
                p2 = new PointF(b.X, b.Y);
                return true;
            }

            var a_num = Convert.ToInt32(a_code, 2);
            var b_num = Convert.ToInt32(b_code, 2);
            if ((a_num & b_num) != 0)
            {
                p1 = new PointF(a.X, a.Y);
                p2 = new PointF(b.X, b.Y);
                return false;
            }
            float k = (b.Y - a.Y) / (b.X - a.X);
            if (k == float.NaN)
            {
                k = float.PositiveInfinity;
            }
            if (a_num == 0)
            {
                Swap(ref a, ref b);
                Swap(ref p1, ref p2);
                a_code = getCode(a); 
                b_code = getCode(b);
                a_num = Convert.ToInt32(a_code, 2);
                b_num = Convert.ToInt32(b_code, 2);
            }
            if (a_num != 0 && b_num == 0)
            {
                PointF p = moveBit(a, k);
                while (!getCode(p).Equals("0000"))
                {
                    p = moveBit(p, k);
                }
                p1 = new PointF(p.X, p.Y);
                p2 = new PointF(b.X, b.Y);
                return true;
            }
                
                float x, y;
                List<PointF> points = new List<PointF>();
            for (int i = 0; i < 2; i++)
            {
                x = p1.X + (1 / k) * (rect.pMax.Y - p1.Y);
                if (x >= rect.minPoint.X && x <= rect.pMax.X)
                {
                    points.Add(new PointF(x, rect.pMax.Y));
                }
                if (points.Count == 2)
                {
                    p1 = points[0];
                    p2 = points[1];
                    return true;
                }
                x = p1.X + (1 / k) * (rect.minPoint.Y - p1.Y);
                if (x >= rect.minPoint.X && x <= rect.pMax.X)
                {
                    points.Add(new PointF(x, rect.minPoint.Y));
                }
                if (points.Count == 2)
                {
                    p1 = points[0];
                    p2 = points[1];
                    return true;
                }
                y = p1.Y + k * (rect.pMax.X - p1.X);
                if (y >= rect.minPoint.Y && y <= rect.pMax.Y)
                {
                    points.Add(new PointF(rect.pMax.X, y));
                }
                if (points.Count == 2)
                {
                    p1 = points[0];
                    p2 = points[1];
                    return true;
                }
                y = p1.Y + k * (rect.minPoint.X - p1.X);
                if (y >= rect.minPoint.Y && y <= rect.pMax.Y)
                {
                    points.Add(new PointF(rect.minPoint.X, y));
                }
                if (points.Count == 2)
                {
                    p1 = points[0];
                    p2 = points[1];
                    return true;
                }
            }
            return false;
        }
        private void Swap(ref PointF a, ref PointF b)
        {
            PointF temp = a;
            a = b;
            b = temp;
        }
        public forSutherlandCohen rect;
        public List<Vector> vectors;
        public List<KeyValuePair<PointF, PointF>> edges;
        public float ScalarMultiply(Vector vec1, Vector vec2)
        {
            return (vec1.A * vec2.A + vec1.B * vec2.B);
        }

        public float VectorMultiply(Vector vec1, Vector vec2)
        {
            return vec1.A * vec2.B - vec1.B * vec2.A;
        }
        public void SetPolygon(List<KeyValuePair<PointF, PointF>> list)
        {
            vectors = new List<Vector>();
            edges = new List<KeyValuePair<PointF, PointF>>(list);
            for (int i = 0; i < list.Count; i++)
            {
                vectors.Add(new Vector(list[i].Key, list[i].Value));
            }

        }

        
        public float getParameter(PointF p, KeyValuePair<PointF, PointF> segment)
        {
            return (p.X - segment.Key.X) / (segment.Value.X - segment.Key.X);
        }
        public float getT(KeyValuePair<PointF, PointF> sides, KeyValuePair<PointF, PointF> segment, ref bool onSameLine)
        {
            var x0e = sides.Key.X;
            var y0e = sides.Key.Y;
            var x1e = sides.Value.X;
            var y1e = sides.Value.Y;


            var x0s = segment.Key.X;
            var y0s = segment.Key.Y;
            var x1s = segment.Value.X;
            var y1s = segment.Value.Y;

            float ks = (y1s - y0s)/(x1s - x0s);
            float ke = (y1e - y0e) / (x1e - x0e);

            float bs = y0s - ks * x0s;
            float be = y0e - ke * x0e;

            var x = (be - bs) / (ks - ke);
            if ((x - x0e) / (x1e - x0e) <= 0 || (x - x0e) / (x1e - x0e) >= 1)
            {
                return -1;
            }
            var te = (x - x0s) / (x1s - x0s);
            if (float.IsNaN(te) && ke == ks && be == bs)
            {
                onSameLine = true;
            }
            return te;
        }
        public void CyrusBeck(PointF a, PointF b, ref float t_1, ref float t_2)
        {
            List<float> inT = new List<float>();
            List<float> outT = new List<float>();
            KeyValuePair<PointF, PointF> segment = new KeyValuePair<PointF, PointF>(a, b);
            var vec_segment = new Vector(segment);
            float t;
            float S;
            bool onSameLine = false;
            for (int i = 0; i < edges.Count; i++)
            {
                t = getT(edges[i], segment, ref onSameLine);
                if (onSameLine) {
                    inT.Add(getParameter(edges[i].Key, segment));
                    outT.Add(getParameter(edges[i].Value, segment));
                    outT.Add(getParameter(edges[i].Key, segment));
                    inT.Add(getParameter(edges[i].Value, segment));
                    onSameLine = false;
                    continue;
                }
                S = -ScalarMultiply(new Vector(-vectors[i].B, vectors[i].A), vec_segment);
                if (t >=0 && t <= 1)
                {
                    if (S > 0)
                    {
                        inT.Add(t);
                    }
                    else if (S < 0)
                    {
                        outT.Add(t);
                    }
                    else
                    {
                        inT.Add(t);
                        outT.Add(t);
                    }
                }
            }
            if (outT.Count == 0 && inT.Count == 0)
            {
                t_1 = -1;
                t_2 = -1;
                return;
            }
            float tin = 0;
            float tout = 1;
            for (int i = 0; i < inT.Count; i++)
            {
                if (tin < inT[i])
                {
                    tin = inT[i];
                }
            }

            for (int i = 0; i < outT.Count; i++)
            {
                if (tout > outT[i])
                {
                    tout = outT[i];
                }
            }
            t_1 = tin;
            t_2 = tout;
        }
    }
}

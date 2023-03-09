using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Controls;


namespace lab3
{
    public class Calculator
    {
        private ParamStruct Params;

        private BackgroundWorker Back { get; set; } = null;

        private bool Success;
        public bool GetSuccess() { return Success; }
        private Exception LastExcept { get; set; } = null;
        public Exception GetException() { return LastExcept; }

        private ResultStruct[] Results;
        public ResultStruct[] GetResults() { return Results; }

        public Calculator(ParamStruct param)
        {
            if (param.XIterCount < 1)
            {
                Success = false;
                LastExcept = new Exception("Некорректные значения.\nПересоздайте класс 'Calculator'");
                return;
            }
            else Success = true;
            Params = new ParamStruct { X0 = param.X0, XIterCount = param.XIterCount, Incr = param.Incr };
            Results = new ResultStruct[param.XIterCount];
        }
        public Calculator(ParamStruct param, BackgroundWorker back)
        {
            if (param.XIterCount < 1)
            {
                Success = false;
                LastExcept = new Exception("Некорректные значения.\nПересоздайте класс 'Calculator'");
                return;
            }
            else Success = true;
            Params = new ParamStruct { X0 = param.X0, XIterCount = param.XIterCount, Incr = param.Incr };
            Results = new ResultStruct[param.XIterCount];
            Back = back;
        }


        public ResultStruct[] Calculate()
        {
            if (!Success)
            {
                return null;
            }

            try
            {
                int i = 0;
                int x = Params.X0;
                do
                {
                    Results[i] = F(x);

                    x += Params.Incr;
                    i++;

                    if (Back != null && i % 10e+0 == 0)
                    {
                        if (Back.CancellationPending)
                        {
                            return Results;
                        }
                        Back.ReportProgress((i * 100) / Results.Length, Results);
                    }
                }
                while (i < Results.Length);
            }
            catch (Exception ex) { LastExcept = ex; return null; }

            return Results;
        }


        private ResultStruct F(int x)
        {
            ResultStruct result = new ResultStruct()
            {
                X = x,
                F1x = F1(x),
                F2x = F2(x),
                F3x = F3(x),
                F4x = F4(x)
            };
            return result;
        }

        private double F1(int x)//exp(x/pi)
        {
            double res = 0;
            try
            {
                res = Math.Exp(x / Math.PI);
                res = Math.Round(res, 2);
            }
            catch (Exception ex)
            {
                if (LastExcept != null)
                    LastExcept = ex;
            }
            if (double.IsNaN(res) || double.IsInfinity(res))
                res = 0;
            return res;
        }
        private double F2(int x)//log4(1-1/(1-x))
        {
            double res = 0;
            try
            {
                res = Math.Log(1 - 1 / (1 - x), 4);
                res = Math.Round(res, 2);
            }
            catch (Exception ex)
            {
                if (LastExcept != null)
                    LastExcept = ex;
            }
            if (double.IsNaN(res) || double.IsInfinity(res))
                res = 0;
            return res;
        }
        private double F3(int x)
        {
            double res = 0;
            try
            {
                if (x > 0)//sh(x^2-ln(x))
                {
                    double lx = Math.Pow(x, 2) - Math.Log(x, Math.E);
                    res = Math.Exp(lx) - Math.Exp(-lx) / 2;
                    res = Math.Round(res, 2);
                }
                else//tg(1/x^2)
                {
                    res = Math.Tan(1 / Math.Pow(x, 2));
                    res = Math.Round(res, 2);
                }
            }
            catch (Exception ex)
            {
                if (LastExcept != null)
                    LastExcept = ex;
            }
            if (double.IsNaN(res) || double.IsInfinity(res))
                res = 0;
            return res;
        }
        private double F4(int x)
        {
            double res = 0;
            try
            {
                double lRes = 0;
                for (int i = 1; i < 10e+6; i++)//до 1 миллиона
                {
                    lRes += 1 / (x + Math.Pow(i, 1 / 2));
                }
                res = lRes;
                res = Math.Round(res, 2);
            }
            catch (Exception ex)
            {
                if (LastExcept != null)
                    LastExcept = ex;
            }

            if (double.IsNaN(res) || double.IsInfinity(res))
                res = 0;
            return res;
        }

    }


    public struct ResultStruct
    {
        public int X;
        public double F1x, F2x, F3x, F4x;
        public double Fx() { return F1x + F2x + F3x + F4x; }
    }
    public struct ParamStruct
    {
        public int X0, XIterCount, Incr;
    }

}

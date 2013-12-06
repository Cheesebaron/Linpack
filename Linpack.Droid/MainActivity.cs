using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Android.OS;
using Environment = System.Environment;

namespace Linpack.Droid
{
    [Activity(Label = "Linpack Benchmark", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const int DefaultRuns = 10;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var cpuCount = Environment.ProcessorCount;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var iterations = FindViewById<EditText>(Resource.Id.iterations);
            var single = FindViewById<Button>(Resource.Id.single);
            var multi = FindViewById<Button>(Resource.Id.multi);
            var mflops = FindViewById<TextView>(Resource.Id.mflops);
            var time = FindViewById<TextView>(Resource.Id.time);
            var normres = FindViewById<TextView>(Resource.Id.normres);
            var precision = FindViewById<TextView>(Resource.Id.precision);

            single.Click += (s, e) =>
            {
                var numberOfRuns = string.IsNullOrEmpty(iterations.Text) ? DefaultRuns : int.Parse(iterations.Text);

                var results = new List<LinpackResult>();
                for (var i = 0; i < numberOfRuns; i++)
                {
                    var l = new Linpack();
                    var result = l.run_benchmark();
                    results.Add(result);
                }
                
                mflops.Text = string.Format("{0}", results.Average(x => x.MFlops));
                time.Text = string.Format("{0}", results.Sum(x => x.Time));
                normres.Text = string.Format("{0}", results.Average(x => x.NormRes));
                precision.Text = string.Format("{0}", results.Average(x => x.Precision));
            };

            multi.Click += (s, e) =>
            {
                var numberOfRuns = string.IsNullOrEmpty(iterations.Text) ? DefaultRuns : int.Parse(iterations.Text);

                var results = new List<List<LinpackResult>>();
                for (var i = 0; i < numberOfRuns; i++)
                {
                    var innerResults = new List<LinpackResult>();
                    Parallel.For(0, cpuCount, x =>
                    {
                        var l = new Linpack();
                        var r = l.run_benchmark();
                        innerResults.Add(r);
                    });
                    results.Add(innerResults);
                }

                var tmpMflops = 0.0;
                var tmpTime = 0.0;
                var tmpNormRes = 0.0;
                var tmpPrecision = 0.0;

                foreach (var result in results)
                {
                    tmpMflops += result.Sum(x => x.MFlops);
                    tmpTime += result.Sum(x => x.Time);
                    tmpNormRes += result.Average(x => x.NormRes);
                    tmpPrecision += result.Average(x => x.Precision);
                }

                mflops.Text = string.Format("{0}", tmpMflops/numberOfRuns);
                time.Text = string.Format("{0}", tmpTime);
                normres.Text = string.Format("{0}", tmpNormRes/numberOfRuns);
                precision.Text = string.Format("{0}", tmpPrecision/numberOfRuns);
            };

            var singleJava = FindViewById<Button>(Resource.Id.single_java);
            var multiJava = FindViewById<Button>(Resource.Id.multi_java);
            var mflopsJava = FindViewById<TextView>(Resource.Id.mflops_java);
            var timeJava = FindViewById<TextView>(Resource.Id.time_java);
            var normresJava = FindViewById<TextView>(Resource.Id.normres_java);
            var precisionJava = FindViewById<TextView>(Resource.Id.precision_java);

            singleJava.Click += (sender, args) =>
            {
                var numberOfRuns = string.IsNullOrEmpty(iterations.Text) ? DefaultRuns : int.Parse(iterations.Text);

                var results = new List<double> {0.0, 0.0, 0.0, 0.0};
                for (var i = 0; i < numberOfRuns; i++)
                {
                    var javaLinpack = new JavaLinpack();
                    var result = javaLinpack.Benchmark();
                    results[0] += result[0];
                    results[1] += result[1];
                    results[2] += result[2];
                    results[3] += result[3];
                }

                mflopsJava.Text = string.Format("{0}", results[0]/numberOfRuns);
                timeJava.Text = string.Format("{0}", results[1]);
                normresJava.Text = string.Format("{0}", results[2]/numberOfRuns);
                precisionJava.Text = string.Format("{0}", results[3]/numberOfRuns);
            };

            multiJava.Click += (sender, args) =>
            {
                var numberOfRuns = string.IsNullOrEmpty(iterations.Text) ? DefaultRuns : int.Parse(iterations.Text);

                var results = new List<double> { 0.0, 0.0, 0.0, 0.0 };

                for (var i = 0; i < numberOfRuns; i ++)
                {
                    var innerResults = new List<double[]>();
                    Parallel.For(0, cpuCount, x =>
                    {
                        var javaLinpack = new JavaLinpack();
                        var result = javaLinpack.Benchmark();
                        innerResults.Add(result);
                    });

                    results[0] += innerResults.Sum(x => x[0]);
                    results[1] += innerResults.Sum(x => x[1]);
                    results[2] += innerResults.Sum(x => x[2]);
                    results[3] += innerResults.Sum(x => x[3]);
                }

                mflopsJava.Text = string.Format("{0}", results[0]/numberOfRuns);
                timeJava.Text = string.Format("{0}", results[1]);
                normresJava.Text = string.Format("{0}", results[2]/numberOfRuns);
                precisionJava.Text = string.Format("{0}", results[3]/numberOfRuns);
            };
        }
    }
}


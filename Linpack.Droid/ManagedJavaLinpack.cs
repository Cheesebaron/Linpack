using System;
using Android.Runtime;

namespace Linpack.Droid
{
    public class JavaLinpack : ManagedJavaLinpack
    {
        public double[] Benchmark()
        {
            return RunBenchmark();
        }
    }


    [Register("dk/ostebaronen/linpack/droid/Linpack", DoNotGenerateAcw = true)]
    public class ManagedJavaLinpack : Java.Lang.Object
    {
        static IntPtr class_ref = JNIEnv.FindClass("dk/ostebaronen/linpack/droid/Linpack");

        protected ManagedJavaLinpack()
        {
        }

        protected ManagedJavaLinpack(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
        }

        protected override Type ThresholdType
        {
            get { return typeof(ManagedJavaLinpack); }
        }

        protected override IntPtr ThresholdClass
        {
            get { return class_ref; }
        }

        static IntPtr id_run_benchmark;

        [Register("run_benchmark", "()[D", "GetRunBenchmarkHandler")]
        public double[] RunBenchmark()
        {
            if (id_run_benchmark == IntPtr.Zero)
                id_run_benchmark = JNIEnv.GetMethodID(class_ref, "run_benchmark", "()[D");
            if (GetType() == ThresholdType)
            {
                var temp = JNIEnv.CallObjectMethod(Handle, id_run_benchmark);
                var javaArray = JNIEnv.GetArray<double>(temp);
                return javaArray;
            }
            else
            {
                var temp = JNIEnv.CallNonvirtualObjectMethod(Handle, ThresholdClass, id_run_benchmark);
                var javaArray = JNIEnv.GetArray<double>(temp);
                return javaArray;
            }
        }

#pragma warning disable 0169
        static Delegate cb_run_benchmark;
        static Delegate GetRunBenchmarkHandler()
        {
            if (cb_run_benchmark == null)
                cb_run_benchmark = JNINativeWrapper.CreateDelegate((Action<IntPtr, IntPtr>)n_RunBenchmark);
            return cb_run_benchmark;
        }

        static void n_RunBenchmark(IntPtr jnienv, IntPtr lrefThis)
        {
            ManagedJavaLinpack __this = Java.Lang.Object.GetObject<ManagedJavaLinpack>(lrefThis, JniHandleOwnership.DoNotTransfer);
            __this.RunBenchmark();
        }
#pragma warning restore 0169
    }
}
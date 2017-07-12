using System;
using System.Linq;
using System.IO;
using CppSharp;
using Xamarin.Android.Tools;
using static System.IO.Path;

namespace MonoEmbeddinator4000
{
    /// <summary>
    /// Contains various path logic for Xamarin.Android
    /// </summary>
    static class XamarinAndroid
    {
        public const string TargetFrameworkVersion = "v7.0";
        public const string MinSdkVersion = "9";
        public const string TargetSdkVersion = "25";
        public const string JavaVersion = "1.8";

        static XamarinAndroid()
        {
            AndroidLogger.Info += AndroidLogger_Info;
            AndroidLogger.Warning += AndroidLogger_Warning;
            AndroidLogger.Error += AndroidLogger_Error;

            AndroidSdk.Refresh();
        }

        static void AndroidLogger_Info(string task, string message)
        {
            Diagnostics.Debug(message);
        }

        static void AndroidLogger_Warning(string task, string message)
        {
            Diagnostics.Warning(message);
        }

        static void AndroidLogger_Error(string task, string message)
        {
            Diagnostics.Error(message);
        }

        static string GetMonoDroidPath()
        {
            string external = Combine(Helpers.FindDirectory("external"), "Xamarin.Android");
            if (Directory.Exists(external))
                return external;

            string binPath = MonoDroidSdk.BinPath;

            //On Windows, it is generally correct, but probe for "lib"
            if (File.Exists(Combine(binPath, "lib")))
                return GetFullPath(MonoDroidSdk.BinPath);

            //On Mac, it is up one directory from BinPath
            return GetFullPath(Combine(MonoDroidSdk.BinPath, ".."));
        }

        static string GetMonoDroidLibPath()
        {
            var libPath = Combine(Path, "lib", "xbuild", "Xamarin", "Android", "lib");
            if (!Directory.Exists(libPath))
                libPath = Combine(Path, "lib");
            return libPath;
        }

        static Lazy<string> path = new Lazy<string>(GetMonoDroidPath);

        public static string Path
        {
            get { return path.Value; }
        }

        static Lazy<string> libraryPath = new Lazy<string>(GetMonoDroidLibPath);

        public static string LibraryPath
        {
            get { return libraryPath.Value; }
        }

        static Lazy<string[]> targetFrameworks = new Lazy<string[]>(() => new[]
        {
            Combine(Path, "lib", "xbuild-frameworks", "MonoAndroid", "v1.0"),
            Combine(Path, "lib", "xbuild-frameworks", "MonoAndroid", "v1.0", "Facades"),
            Combine(Path, "lib", "xbuild-frameworks", "MonoAndroid", TargetFrameworkVersion),
            Combine(Path, "lib", "xbuild-frameworks", "MonoAndroid", "v2.3"), //Mono.Android.Export.dll is here
        });

        public static string[] TargetFrameworkDirectories
        {
            get { return targetFrameworks.Value; }
        }

        /// <summary>
        /// Finds a Xamarin.Android assembly in v1.0, v1.0/Facades, or TargetFrameworkVersion
        /// NOTE: that this will also work for mono.android.jar
        /// </summary>
        public static string FindAssembly(string assemblyName)
        {
            foreach (var dir in TargetFrameworkDirectories)
            {
                string assemblyPath = Combine(dir, assemblyName);
                if (File.Exists(assemblyPath))
                    return assemblyPath;
            }

            throw new FileNotFoundException("Unable to find assembly!", assemblyName);
        }

        static Lazy<int> apiLevel = new Lazy<int>(() => AndroidSdk.GetInstalledPlatformVersions().Select(m => m.ApiLevel).Max());

        /// <summary>
        /// Right now we are choosing the max API level installed
        /// </summary>
        public static int ApiLevel
        {
            get { return apiLevel.Value; }
        }

        static Lazy<string> platformDirectory = new Lazy<string>(() => AndroidSdk.GetPlatformDirectory(ApiLevel));

        /// <summary>
        /// Gets the platform directory based on the max API level installed
        /// </summary>
        public static string PlatformDirectory
        {
            get { return platformDirectory.Value; }
        }
    }
}
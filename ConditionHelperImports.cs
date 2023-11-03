using MonoMod.ModInterop;
using System;

namespace Celeste.Mod.AchievementHelper {
    [ModImportName("ConditionHelper")]
    public static class ConditionHelperImports {

        public static Action<string> ConditionChanged;

        public static Func<string, Action, int> WatchConditions;

        public static Action<int> RemoveCallback;

        public static Func<string, bool> EvaluateConditionExpression;
    }
}

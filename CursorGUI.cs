using HutongGames.PlayMaker;

namespace OASIS
{
    public static class CursorGUI
    {
        public static bool assemble
        {
            get => GUIassemble.Value;
            set => GUIassemble.Value = value;
        }
        public static bool buy
        {
            get => GUIbuy.Value;
            set => GUIbuy.Value = value;
        }
        public static bool disassemble
        {
            get => GUIdisassemble.Value;
            set => GUIdisassemble.Value = value;
        }
        public static bool drive
        {
            get => GUIdrive.Value;
            set => GUIdrive.Value = value;
        }
        public static bool passenger
        {
            get => GUIpassenger.Value;
            set => GUIpassenger.Value = value;
        }
        public static bool use
        {
            get => GUIuse.Value;
            set => GUIuse.Value = value;
        }
        public static string gear
        {
            get => GUIgear.Value;
            set => GUIgear.Value = value;
        }
        public static string interaction
        {
            get => GUIinteraction.Value;
            set => GUIinteraction.Value = value;
        }
        public static string subtitle
        {
            get => GUIsubtitle.Value;
            set => GUIsubtitle.Value = value;
        }
        static readonly FsmBool GUIassemble = FsmVariables.GlobalVariables.FindFsmBool("GUIassemble");
        static readonly FsmBool GUIbuy = FsmVariables.GlobalVariables.FindFsmBool("GUIbuy");
        static readonly FsmBool GUIdisassemble = FsmVariables.GlobalVariables.FindFsmBool("GUIdisassemble");
        static readonly FsmBool GUIdrive = FsmVariables.GlobalVariables.FindFsmBool("GUIdrive");
        static readonly FsmBool GUIpassenger = FsmVariables.GlobalVariables.FindFsmBool("GUIpassenger");
        static readonly FsmBool GUIuse = FsmVariables.GlobalVariables.FindFsmBool("GUIuse");
        static readonly FsmString GUIgear = FsmVariables.GlobalVariables.FindFsmString("GUIgear");
        static readonly FsmString GUIinteraction = FsmVariables.GlobalVariables.FindFsmString("GUIinteraction");
        static readonly FsmString GUIsubtitle = FsmVariables.GlobalVariables.FindFsmString("GUIsubtitle");
    }
}
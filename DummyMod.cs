using MSCLoader;

namespace OASIS
{
    class DummyMod : Mod
    {
        public override string ID => "OASIS_DummyMod";

        public override string Version => null;

        public override string Author => null;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnMenuLoad, () =>
            {
                ModUI.ShowCustomMessage("OASIS is not a mod. Move it to the References folder or delete it and let MSCLoader download the latest version automatically.", "READ ME", new MsgBoxBtn[]
                {
                    ModUI.CreateMessageBoxBtn("I will", () => { }, true)
                });
            });
        }
    }
}
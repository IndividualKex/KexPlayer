using System;

namespace KexInteract {
    [Flags]
    public enum InteractionMask : byte {
        None = 0,
        Layer1 = 1 << 0,
        Layer2 = 1 << 1,
        Layer3 = 1 << 2,
        Layer4 = 1 << 3,
        Layer5 = 1 << 4,
        Layer6 = 1 << 5,
        Layer7 = 1 << 6,
        Layer8 = 1 << 7,
    }

    [Flags]
    public enum ControlMask : byte {
        Fire = 1 << 0,
        AltFire = 1 << 1,
        Interact = 1 << 2,
        AltInteract = 1 << 3,
        Action1 = 1 << 4,
        Action2 = 1 << 5
    }
}

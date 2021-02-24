using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu ("Custom/HueShift")]
public sealed class HueShift : VolumeComponent, IPostProcessComponent {
    public ClampedFloatParameter percentage = new ClampedFloatParameter (0f, 0f, 1f);

    public bool IsActive () {
        return percentage.value > 0f;
    }

    public bool IsTileCompatible () => true;
}

Shader "Custom/Stencil/StencilWriter"
{
    Properties
    {
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "Queue" = "Geometry-100"
        }

        Pass
        {
            Blend Zero One

            Stencil 
            {
                Ref [_StencilID]
                Comp Always
                Pass replace
                Fail Keep
            }
        }
    }
}

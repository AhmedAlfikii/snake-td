Shader "Custom/Stencil/WriteDepthOnly"
{  
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-50"
        }
        ColorMask 0
        ZWrite on
      
        Pass { }
    }
}
// Compile with: .\fxc TintShader.hlsl /E main /T ps_3_0 /O3 /Fo TintShader.cso
sampler2D MipImage : register(s0);
float4 TintWhite : register(c0);
float4 TintBlack : register(c1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 c = tex2D(MipImage, uv);
    
    c.rgb *= TintWhite.rgb;
    c.rgb += (1 - c.rgb) * TintBlack.rgb;
    
    return c;
}
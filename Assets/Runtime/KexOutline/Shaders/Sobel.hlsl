void Sobel_float(float2 uv, float2 size, out float Out)
{
    size /= _ScreenParams.xy;
    float3x3 viewMatrix = (float3x3)UNITY_MATRIX_V;

    float depths[9];
    float3 normals[9];

    int index = 0;
    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            float2 sampleUv = uv + float2(x, y) * size;
            depths[index] = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(sampleUv), _ZBufferParams);
            normals[index] = mul(viewMatrix, SHADERGRAPH_SAMPLE_SCENE_NORMAL(sampleUv));
            index++;
        }
    }

    float depthGx = (depths[0] * -1) + (depths[2] * 1) + (depths[3] * -2) + (depths[5] * 2) + (depths[6] * -1) + (depths[8] * 1);
    float depthGy = (depths[0] * -1) + (depths[6] * 1) + (depths[3] * -2) + (depths[5] * 2) + (depths[1] * -1) + (depths[7] * 1);
    float depthG = sqrt(depthGx * depthGx + depthGy * depthGy);

    float normalGx = (normals[0].x * -1) + (normals[2].x * 1) + (normals[3].x * -2) + (normals[5].x * 2) + (normals[6].x * -1) + (normals[8].x * 1);
    float normalGy = (normals[0].y * -1) + (normals[6].y * 1) + (normals[3].y * -2) + (normals[5].y * 2) + (normals[1].y * -1) + (normals[7].y * 1);
    float normalG = sqrt(normalGx * normalGx + normalGy * normalGy);

    float G = depthG * 1.0 + normalG * 0.5;

    Out = smoothstep(0.1, 0.3, G);
}

/*V vershinnom shader rabotaem s odnoi vershinoi (dlya parallelisma)*/
/*Zapuskaetsa dlya vsex vershin, kotorie bili peredani opengl-y*/

/*kakay versia GL SL (GL Shading Language) ispolzuetsya*/
    #version 330 core 

/*in - vxodnaya peremennaya*/
    layout (location = 0) in vec3 position; //coord vershini (odnoi)
    layout (location = 1) in vec3 vNormal;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;
            
//Output data of this shader:
//Fragment position and recalculated normal
    out vec3 FragPos;
    out vec3 Normal;
            
    void main()
    {
        vec4 worldCoordinates = vec4(position, 1.0) * model;

/*gl_Position - vixodnaya peremenaya*/
/*V gl_Position skladivaetsa vixod s versh stadii*/
/*To, chto v nee zapis - budet ispol sled shag conveera*/
/*Pos umnozh na dvizhenie, ...*/
        gl_Position = worldCoordinates * view * projection;
                
//And passing into fragment shader our this position in our world's coordinates
        FragPos = vec3(worldCoordinates);

//Passing recalculated normals into fragment shader
//(our model matrix includes rotation, and normals
// in a world space become incorrect after that)
        Normal = vNormal * mat3(transpose(inverse(model)));
    }
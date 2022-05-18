/*V vershinnom shader rabotaem s odnoi vershinoi (dlya parallelisma)*/
/*Zapuskaetsa dlya vsex vershin, kotorie bili peredani opengl-y*/

    #version 330 core /*kakay versia GL SL (GL Shading Language) ispolzuetsya*/

/*in - vxodnaya peremennaya*/
    layout (location = 0) in vec3 aPos; //coord vershini
    layout (location = 1) in vec3 aNormal;
    layout (location = 2) in vec2 aTexCoords;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;

    out vec3 Normal;                                                    /*vector normali - edinich vektor, perpendikulyarniy poverxnosti, postroennoi na dannoi vershine*/                              
    out vec3 FragPos;                                                   /*position tekushego fragmenta*/
    out vec2 TexCoords;

    void main()
    {
/*gl_Position - vixodnaya peremenaya*/
/*V gl_Position skladivaetsa vixod s versh stadii*/
/*To, chto v nee zapis - budet ispol sled shag conveera*/
/*Pos umnozh na dvizh, ...*/
        gl_Position = vec4(aPos, 1.0) * model * view * projection;  

        FragPos = vec3(vec4(aPos, 1.0) * model);                        /*preobrazovivaem k mirovim coord*/
        Normal = aNormal * mat3(transpose(inverse(model)));
        TexCoords = aTexCoords;
    }
    #version 330                            /*kakay versia GL SL (GL Shading Language) ispolzuetsya*/
   
    struct Material
    {
        vec3 ambient;                   /*1ya komponenta osvesheniya - fonovoe: ispolzuem nekotoruy const okruzhaushego osvesheniya, kotoraya budet pridavat obekty ottenok (Primer: ne bivaet polnost temn predm)*/
        vec3 diffuse;                   /*2ay komponenta osvesheniya - difusnoe*/
        vec3 specular;
        float shininess;
    };

    struct Light 
    {
        vec3  position;                 /*polozhenie prozektora*/
        vec3  direction;                /*napravlenie prozektora*/
        float cutOff;                   /*ugol otsecheniya (opredelyaet radius podsvetki)*/
        float outerCutOff;              /*ugol mezhdu napravleniem prozektora i napravleniem is prozektora do fragmenta*/
            
        vec3 ambient;                   /*1ya komponenta osvesheniya - fonovoe: ispolzuem nekotoruy const okruzhaushego osvesheniya, kotoraya budet pridavat obekty ottenok (Primer: ne bivaet polnost temn predm)*/
        vec3 diffuse;                   /*2ay komponenta osvesheniya - difusnoe*/
        vec3 specular;

/*kf dlya oslableniya intensivnosti osvesheniya s rastoyniem -efect zatuxaniya*/            
        float constant;
        float linear;
        float quadratic;
    };

    in vec3 FragPos;                     /*position tekushego fragmenta*/ 
    in vec3 Normal;                      /*vector normali - edinich vektor, perpendikulyarniy poverxnosti, postroennoi na dannoi vershine*/ 

/*uniform - "zadana odnoi neizmenyausheysa peremennoi"*/            
    uniform Light light;
    uniform Material material;
    uniform vec3 viewPos; 

/*Vixod - cvet piksylya na ekrane*/
/*esli odin treugoln - eto pikseli, kotorie predstavlyayt treugoln*/
    out vec4 FragColor;
            
    void main()
    {

//1. fonovaya componenta osvesheniya
        vec3 ambient = light.ambient * material.ambient;

//2.diffusznaya komponenta osvesheniya
        vec3 norm = normalize(Normal);
        //(Direction of the light - a difference vector between the light's position and the fragment's position)
        vec3 lightDir = normalize(light.position - FragPos);                                                    //raznost pozitsii istochnika sveta i fragmenta
        float diff = max(dot(norm, lightDir), 0.0);                                                             //velichina vozdeyistviya diffusnogo osvesheniya na tekushiy fragment - skalarnoe proizvedenie: norm i lightDir
        vec3 diffuse = light.diffuse * (diff * material.diffuse);                                               //velichina vozdeyistviya * na cvet istochnika sceta //Poluchili komponent diffusn osvesheniya, kotora stanovitsa temnee s uvelich ugla mezdhu vektorami
            
//3. Specular component
        vec3 viewDir = normalize(viewPos - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = light.specular * (spec * material.specular);
    
//4. zatuxanie
        float distance    = length(light.position - FragPos);                                                               //rastoyanie ot fragmenta do istochnika sveta 
        float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));     //kf zatuxaniya

//5. intensivnost        
        float theta     = dot(lightDir, normalize(-light.direction));                                                       //viichislayem skalyrnoe proizvedenie mezdy vectorom fragmenta i vectorom napravleniya
        float epsilon   = light.cutOff - light.outerCutOff;                                                                 //dlya koef intensivnosti
        float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);                                           //Intensivnost (ispol, chtobi sgladit okruzn osvesheniya) //clamp ogranichiv znac 2ogo i 3ego param
        
        ambient  *= attenuation;                                                                                            //okr sreda - prostranstvo, gde ne svetit prozhektor - ne domnozh na intensivn
        diffuse  *= attenuation * intensity;
        specular *= attenuation * intensity;
            
//itogoviy cvet
        FragColor = vec4((ambient + diffuse + specular), 1.0);
    }           
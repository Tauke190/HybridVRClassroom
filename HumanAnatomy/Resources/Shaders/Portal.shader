Shader "Custom/Portal"
{
  
    SubShader
    {
		Zwrite off
		ColorMask 0 
	Stencil
	{
		Ref 1 
		Pass replace 

    }
       
        Pass
        {
           
		
        }
    }
}

import { FormEvent, useRef } from "react";
import { CredentialResponse, GoogleLogin } from "@react-oauth/google";
import RouteTo from "./RouteTo";
// import AuthUserLoginDto from "../common/models/AuthUserLoginDto";
import GenResponse from "../common/config/GenResponse";
import { UserAccountCreateDTO } from "../common/models/UserLoginResponse";

interface RegisterWidgetProps<T> {
    onSuccess?: (credentialResponse: GenResponse<T>) => void;
    onError?: (err: GenResponse<T>) => void;
    allowSocialLogin?: boolean;
    apiUrl: string;
    googleApiUrl: string;
    signInRoute?: string;
    loadingText?: string;
}

const RegisterWidget = <T,>({onError,onSuccess,apiUrl,googleApiUrl,signInRoute,allowSocialLogin=true, loadingText="Processing..."}: RegisterWidgetProps<T>) => {

//   const [respData, setRespData] = React.useState<GenResponse<T>>(new GenResponse<T>());
  const btnRef = useRef<HTMLButtonElement>(null);
  const LoginSuccess = async (credentialResponse: CredentialResponse) => {
    if (btnRef.current)
    {
        btnRef.current!.disabled = true;
        btnRef.current!.innerText = loadingText;
    }
    if (credentialResponse.credential) {
        //TODO: Call backend AP and retrieve consisite UserLoginResponse details
        try{
            const objResp = await fetch(googleApiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({token: credentialResponse.credential})
            });
        //   if(onSuccess){onSuccess(GenResponse.Result<T>(credentialResponse);}
          if(objResp?.status){
            const objRespData = (await objResp.json()) as GenResponse<T>;
            if(objRespData.isSuccess && objRespData.result){
                // alert(JSON.stringify(objRespData));
                if(onSuccess){onSuccess(GenResponse.Result(objRespData.result));}
            }else{
                if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Login Failed"));}
            }
            }
        else{
            alert("Login failed");
            if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Login Failed"));}
        }
        }
        catch{
            // alert('An error occured. Kindly retry again.');
            if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Login Failed"));}
        }finally{
            if (btnRef.current) {
                btnRef.current.disabled = false;
                btnRef.current.innerText = "Sign in";
            }
        }
    }
  };

  const LoginError = () => {
    if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Login Failed"));}
  };
  const LoginSubmitForm =async (evt: FormEvent)=>{
    evt.preventDefault();
    if (!btnRef.current) return;
    btnRef.current!.disabled = true;
    btnRef.current!.innerText = loadingText;
    const allFormData = new FormData(document.getElementById("frmLogin") as HTMLFormElement);
    const jsonEntries = {...Object.fromEntries(allFormData.entries())} as UserAccountCreateDTO;
    try{
        const objResp = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(jsonEntries)
        });
        if(objResp?.status && objResp.ok){
            const objRespData = (await objResp.json()) as GenResponse<T>;
            if(objRespData.isSuccess && objRespData.result){
                // alert(JSON.stringify(objRespData));
                if(onSuccess){onSuccess(GenResponse.Result(objRespData.result));}
            }else{
                // alert(objRespData.error ?? objRespData.message);
                if(onError){onError(GenResponse.Failed<T>(null as unknown as T,objRespData.error ?? 'Registration failed'));}
            }
        }else{
            alert("Registration failed");
            if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Registration Failed"));}
        }
    }catch{
        alert('An error occured. Kindly retry again.');
        if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Registration Failed"));}
    }finally{
        if (btnRef.current) {
            btnRef.current.disabled = false;
            btnRef.current.innerText = "Sign in";
        }
    }
  }
  return (
    <div className="bg-white p-10 rounded-xl shadow-lg border border-gray-200 w-[700px] md:mx-auto ">
      <form id="frmLogin" className="flex flex-col gap-4" onSubmit={LoginSubmitForm}>  
        <div>
            <h2 className="text-center font-bold">Register</h2>
            <section className="text-center text-xs text-gray-400">Sign up to continue</section>
        </div>
        {allowSocialLogin && (
            <div id="socialAuth" className="flex justify-center gap-4">
                <GoogleLogin onSuccess={(credentialResponse) => LoginSuccess(credentialResponse)} onError={() => {LoginError();}} />
            </div>
        )}
        <div id="dvRegister" className="mt-4 flex flex-col gap-4">
            <div className="flex gap-4">
                <div className="flex flex-col flex-grow gap-1">
                    <label htmlFor="firstName" className="text-sm font-semibold text-gray-500">First name</label>
                    <input type="text" minLength={3} id="firstName" name="firstName" className="outline-none border-[2px] rounded-md px-2 py-1 text-sm focus:shadow-lg" placeholder="First name" required/>
                </div>
                <div className="flex flex-col flex-grow gap-1">
                    <label htmlFor="lastName" className="text-sm font-semibold text-gray-500">Last name</label>
                    <input type="text" minLength={3} id="lastName" name="lastName" placeholder="Last name" className="outline-none border-[2px] rounded-md px-2 py-1 text-sm focus:shadow-lg" required/>
                </div>
            </div>
            <div id="dvEmail" className="mt-2 flex flex-col gap-4">
                <div className="flex flex-col gap-1">
                    <label htmlFor="email" className="text-sm font-semibold text-gray-500">Email address</label>
                    <input type="email" id="email" name="email" className="outline-none border-[2px] rounded-md px-2 py-1 text-sm focus:shadow-lg" placeholder="Enter your email address" required/>
                </div>
            </div>
            <div id="dvPassword" className="mt-2 flex gap-4">
                <div className="flex flex-col flex-grow gap-1">
                    <label htmlFor="password" className="text-sm font-semibold text-gray-500">Password</label>
                    <input type="password" id="password" name="password" className="outline-none border-[2px] rounded-md px-2 py-1 text-sm focus:shadow-lg" required/>
                </div>
                <div className="flex flex-col flex-grow gap-1">
                    <label htmlFor="confirmPassword" className="text-sm font-semibold text-gray-500">Confirm password</label>
                    <input type="password" id="confirmPassword" name="confirmPassword" className="outline-none border-[2px] rounded-md px-2 py-1 text-sm focus:shadow-lg" required/>
                </div>
            </div>
            <div>
              <button ref={btnRef} id="btnSubmit" className="bg-gray-800 text-white rounded outline-none px-2 py-1 w-full text-md">Register</button>
            </div>
        </div>
      </form>
      {
        signInRoute && (
            <section id="dvSignup">
                <div className="text-center mt-4">
                    <span className="text-xs font-semibold text-gray-500">Already have an account?</span>
                    <RouteTo to={signInRoute} className="text-blue-600 text-xs">&nbsp;Sign in</RouteTo>
                </div>
            </section>
        )
      }
      
    </div>
  );
};

export default RegisterWidget;

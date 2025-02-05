import { FormEvent, useRef } from "react";
import { CredentialResponse, GoogleLogin } from "@react-oauth/google";
import RouteTo from "./RouteTo";
import AuthUserLoginDto from "../common/models/AuthUserLoginDto";
import GenResponse from "../common/config/GenResponse";
// import { UserLoginResponse } from "../common/models/UserLoginResponse";

interface LoginWidgetProps<T> {
    onSuccess?: (credentialResponse: GenResponse<T>) => void;
    onError?: (err: GenResponse<T>) => void;
    allowSocialLogin?: boolean;
    apiUrl: string;
    googleApiUrl: string;
    signUpRoute?: string;
    loadingText?: string;
}

const LoginWidget = <T,>({onError,onSuccess,apiUrl,googleApiUrl,signUpRoute,allowSocialLogin=true, loadingText="Processing..."}: LoginWidgetProps<T>) => {

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
    const jsonEntries = {...Object.fromEntries(allFormData.entries()), socialLogin :{isSocialLogin: false}} as AuthUserLoginDto;
    try{
        const objResp = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(jsonEntries)
        });
        if(objResp?.status){
            const objRespData = (await objResp.json()) as GenResponse<T>;
            if(objRespData.isSuccess && objRespData.result){
                // alert(JSON.stringify(objRespData));
                if(onSuccess){onSuccess(GenResponse.Result(objRespData.result));}
            }else{
                // alert(objRespData.error ?? objRespData.message);
                if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Login Failed"));}
            }
        }else{
            alert("Login failed");
            if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Login Failed"));}
        }
    }catch{
        alert('An error occured. Kindly retry again.');
        if(onError){onError(GenResponse.Failed<T>(null as unknown as T,"Login Failed"));}
    }finally{
        if (btnRef.current) {
            btnRef.current.disabled = false;
            btnRef.current.innerText = "Sign in";
        }
    }
  }
  return (
    <div className="bg-white p-10 rounded-xl shadow-lg border border-gray-200 w-[400px] md:mx-auto">


      <form id="frmLogin" className="flex flex-col gap-4" onSubmit={LoginSubmitForm}>  
        <div>
            <h2 className="text-center font-bold">Sign in </h2>
            <section className="text-center text-xs text-gray-400">Sign in to continue</section>
        </div>
        {allowSocialLogin && (
            <div id="socialAuth" className="flex justify-center gap-4 mt-4">
                <GoogleLogin onSuccess={(credentialResponse) => LoginSuccess(credentialResponse)} onError={() => {LoginError();}} />
            </div>
        )}
        <div id="dvLogin" className="mt-4 flex flex-col gap-4">
            <div className="flex flex-col gap-1">
                <label htmlFor="email" className="text-sm font-semibold text-gray-500">Email address</label>
                <input type="email" id="email" name="email" className="outline-none border-[2px] rounded-md px-2 py-1 text-sm focus:shadow-lg" placeholder="Enter your email address" required/>
            </div>
            <div className="flex flex-col gap-1">
                <label htmlFor="password" className="text-sm font-semibold text-gray-500">Password</label>
                <input type="password" id="password" name="password" className="outline-none border-[2px] rounded-md px-2 py-1 text-sm focus:shadow-lg" required/>
            </div>
            <div>
                <button ref={btnRef} id="btnSubmit" className="bg-gray-800 text-white rounded outline-none px-2 py-1 w-full text-md">Sign in</button>
            </div>
        </div>
      </form>
      {
        signUpRoute && (
            <section id="dvSignup">
                <div className="text-center mt-4">
                    <span className="text-xs font-semibold text-gray-500">Don't have an account?</span>
                    <RouteTo to={signUpRoute} className="text-blue-600 text-xs">&nbsp;Sign up</RouteTo>
                </div>
            </section>
        )
      }
      
    </div>
  );
};

export default LoginWidget;

import { useEffect } from "react";
import React from "react";
import MetaTag from "../../../_components/MetaTag";
import { useNavigate } from "react-router-dom";
import LoginWidget from "../../../_components/LoginWidget";
import { UserLoginResponse } from "../../../common/models/UserLoginResponse";
import GenResponse from "../../../common/config/GenResponse";
import { useAppStore } from "../../../common/services/appservices";
import BlogAppRoutes from "../../BlogAppRoutes";
import appsettings from "../../../common/config/appsettings";
import avatar from '../../../../assets/images/avatar.png';


const LoginPage: React.FC = () => {

  const {authService,commonService} = useAppStore();
  const navigate = useNavigate();


  useEffect(() => {
      authService.IsSignedIn()
      .then((resp: GenResponse<boolean>) => {
        if (resp && resp.isSuccess && resp.result) {
          navigate(BlogAppRoutes().dashboard.home.parentRoute);
        }else{
          commonService.removeLocalStorage(appsettings.token.authToken);
        }
      })
      .catch((err: GenResponse<boolean>) => {console.error(`error: ${err.result}`);});
  }, [authService, commonService, navigate]);

  const LoginSuccess =(credResp: GenResponse<UserLoginResponse>)=>{
    if(credResp.result && (credResp.result.picture === undefined || credResp.result.picture === null || credResp.result.picture.trim() === '')) {
      credResp.result.picture = avatar;
    }
    const credObjResp= JSON.stringify(credResp);
    console.log(credObjResp);
    commonService.setLocaStoragae(appsettings.token.authToken, credObjResp);
    navigate(BlogAppRoutes().dashboard.home.parentRoute);
  }

  const LoginError =(err: GenResponse<UserLoginResponse>)=>{
      alert(`Error: ${err.error}`);
  }

  return (
    <>
      <MetaTag title="Login" />
      <div className="flex items-center justify-center h-[calc(100vh-80px)]">
        <LoginWidget<UserLoginResponse> apiUrl="http://localhost:5050/api/Auth/Login" googleApiUrl={`http://localhost:5050/api/Auth/GoogleLogin`} onSuccess={LoginSuccess} onError={LoginError} signUpRoute={'/register'} />
      </div>
    </>
  );
};

export default LoginPage;

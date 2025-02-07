import React, { useEffect } from "react";
import MetaTag from "../../../_components/MetaTag";
import RegisterWidget from "../../../_components/RegisterWidget";
import { UserLoginResponse } from "../../../common/models/UserLoginResponse";
import { useNavigate } from "react-router-dom";
import appsettings from "../../../common/config/appsettings";
import GenResponse from "../../../common/config/GenResponse";
import { useAppStore } from "../../../common/services/appservices";
import BlogAppRoutes from "../../BlogAppRoutes";

const RegisterPage: React.FC = () => {
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
      <MetaTag title="Register" />
      <div className="flex items-center justify-center h-[calc(100vh-80px)]">
        <RegisterWidget<UserLoginResponse> apiUrl="http://localhost:5050/api/Auth/register" googleApiUrl={`http://localhost:5050/api/Auth/GoogleLogin`} onSuccess={LoginSuccess} onError={LoginError} signInRoute={'/login'} />
      </div>
    </>
  );
};

export default RegisterPage;

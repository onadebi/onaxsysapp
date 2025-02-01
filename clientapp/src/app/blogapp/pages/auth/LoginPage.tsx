import { SignIn, useAuth } from "@clerk/clerk-react";
import React from "react";
import BlogAppRoutes from "../../BlogAppRoutes";
import MetaTag from "../../../_components/MetaTag";
import { useNavigate } from "react-router-dom";
import LoginWidget from "../../../_components/LoginWidget";


const LoginPage: React.FC = () => {
  const { isSignedIn } = useAuth();
  const navigate = useNavigate();

  if (isSignedIn) {
    navigate(BlogAppRoutes().dashboard.home.parentRoute);
  }


  return (
    <>
      <MetaTag title="Login" />
      <div className="flex items-center justify-center h-[calc(100vh-80px)]">
        <LoginWidget apiUrl="http://localhost:5050/api/Auth/Login" onSuccess={(credResp)=> console.log(`${JSON.stringify(credResp)}`)}/>
        <SignIn
          signUpUrl={BlogAppRoutes().public.register.parentRoute}
          fallbackRedirectUrl={`${BlogAppRoutes().homebase}${
            BlogAppRoutes().dashboard.home.parentRoute
          }`}
        />
      </div>
    </>
  );
};

export default LoginPage;

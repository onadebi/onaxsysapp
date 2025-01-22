import { SignIn, useAuth } from "@clerk/clerk-react";
import React from "react";
import BlogAppRoutes from "../../BlogAppRoutes";
import MetaTag from "../../../_components/MetaTag";
import { useNavigate } from "react-router-dom";

const LoginPage: React.FC = () => {
  const {isSignedIn} = useAuth();
  const navigate = useNavigate();
  if(isSignedIn){
    navigate(BlogAppRoutes().dashboard.home.parentRoute);
  }
  return (
    <>
      <MetaTag title="Login" />
      <div className="flex items-center justify-center h-[calc(100vh-80px)]">
        <SignIn signUpUrl={BlogAppRoutes().public.register.parentRoute} 
        fallbackRedirectUrl={`${BlogAppRoutes().homebase}${BlogAppRoutes().dashboard.home.parentRoute}`} />
      </div>
    </>
  );
};

export default LoginPage;

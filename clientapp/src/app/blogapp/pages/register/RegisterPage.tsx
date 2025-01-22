import React from "react";
import { SignUp } from "@clerk/clerk-react";
import BlogAppRoutes from "../../BlogAppRoutes";
import MetaTag from "../../../_components/MetaTag";

const RegisterPage: React.FC = () => {
  return (
    <>
      <MetaTag title="Register" />
      <div className="flex items-center justify-center h-[calc(100vh-80px)]">
        <SignUp signInUrl={BlogAppRoutes().public.login.parentRoute} />
      </div>
    </>
  );
};

export default RegisterPage;

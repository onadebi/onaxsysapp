import React, { useEffect } from "react";
import MetaTag from "../../../_components/MetaTag";
import FormModal from "../../../_components/FormModal";
import { UserLoginResponse, UserLoginResponseUpdateDTO } from "../../../common/models/UserLoginResponse";
import { hasPermission } from "../../../common/config/auth";
import { useAppStore } from "../../../common/services/appservices";
import UserProfile from "../../../_components/UserProfile";

const Dashboard: React.FC = () => {
  
  const {authService} = useAppStore();
  const [, setUserObj] = React.useState<UserLoginResponse|null>(null);
  useEffect(() => {
    const userDetails = authService.UserProfile();
    if(userDetails.isSuccess && userDetails.result){
      userDetails.result.roles.push(...["guest","admin"]);
      hasPermission(userDetails.result,["view:dashboard","view:mecha"]);
      setUserObj(userDetails.result);
    }
  }, [authService]);
  return (
    <>
      <MetaTag title={"Dashboard"} />
      <div>
        <h3>Dashboard</h3>
        <p>Welcome to the dashboard</p>
          <FormModal<UserLoginResponseUpdateDTO> table="userProfile" type="create" 
          title="Create new user" data={{} as UserLoginResponseUpdateDTO}/>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <UserProfile/>
      </div>
    </>
  );
};

export default Dashboard;

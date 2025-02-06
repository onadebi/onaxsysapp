import React from "react";
import MetaTag from "../../../_components/MetaTag";
import FormModal from "../../../_components/FormModal";
import { UserLoginResponseUpdateDTO } from "../../../common/models/UserLoginResponse";

const Dashboard: React.FC = () => {
  return (
    <>
      <MetaTag title={"Dashboard"} />
      <div>
        <h3>Dashboard</h3>
        <p>Welcome to the dashboard</p>
        <p>
          <FormModal<UserLoginResponseUpdateDTO> table="userProfile" type="create" 
          title="Create new user" data={{} as UserLoginResponseUpdateDTO}/>
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
        <p>
          Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa aspernatur, deleniti vel voluptas labore ducimus hic nobis cupiditate adipisci! Culpa assumenda suscipit aspernatur vero molestiae recusandae deserunt amet, eligendi quo.
        </p>
      </div>
    </>
  );
};

export default Dashboard;

import { useLocation } from "react-router-dom";
import AppRoutes from "../../routes/AppRoutes";
import { appServices , useAppStore } from "../common/services/appservices";
import RouteTo from "./RouteTo";
import { useEffect, useState } from "react";
import { hasPermission } from "../common/config/auth";


const LogOut = async (evt: React.MouseEvent<HTMLAnchorElement, MouseEvent>) => {
    evt.preventDefault();
    const confirmLogout = window.confirm('Are you sure you want to logout?');
    if(!confirmLogout){return;}
    const objResp = await appServices.authService.logout();
    if(objResp){window.location.reload();}
  }

const menuItems = [
  {
    title: "MENU",
    items: [
      {
        icon: "/assets/images/home.png",
        label: "Home",
        href: AppRoutes().dashboard.home.parentRoute,
        permissions: ["view:dashboard"],
      },
      {
        icon: "/assets/images/assignment.png",
        label: "Compose",
        href: AppRoutes().dashboard.write.parentRoute,
        permissions: ["view:dashboard"],
      },
      {
        icon: "/assets/images/class.png",
        label: "Upload Test",
        href: AppRoutes().dashboard.test.parentRoute,
        permissions: ["view:dashboard"],
      },
    ],
  },
  {
    title: "OTHER",
    items: [
      {
        icon: "/assets/images/profile.png",
        label: "Profile",
        href: "/profile",
        permissions: ["view:dashboard"],
      },
      {
        icon: "/assets/images/setting.png",
        label: "Settings",
        href: "/settings",
        permissions: ["view:dashboard"],
      },
      {
        icon: "/assets/images/logout.png",
        label: "Logout",
        href: "/logout",
        permissions: ["view:dashboard"],
        onClick: (evt: React.MouseEvent<HTMLAnchorElement, MouseEvent>) => LogOut(evt)
      },
    ],
  },
];

const Menu: React.FC = () => {

    const {pathname} = useLocation();
    const [path, setPath] = useState(pathname);
    const {authService} = useAppStore();
    useEffect(()=>{
        setPath(pathname);
    },[pathname]);

    
  return (
    <div className="">
      {menuItems.map((menu) => (
        <div className="" key={menu.title}>
          <span className="hidden lg:block text-gray-400 font-light my-2 border-l-[4px] border-l-gray-400 pl-2 ml-1">{menu.title}</span>
          {menu.items.map((item, index) => {
           if(hasPermission(authService.UserProfile().result!, item.permissions)){
              return (
                <RouteTo to={item.href} className={`flex items-center justify-center border-b-[1px] border-black-300 lg:justify-start gap-2 text-gray-500 py-2 md:px-2 rounded-md hover:bg-onaxSky ${path === item.href ? 'bg-onaxPurple':''}`} key={index}>
                  <img src={item.icon} alt={item.label} title={item.label} width={20} height={20} />
                  <span className="hidden lg:block whitespace-nowrap" onClick={item.onClick}>{item.label}</span>
                </RouteTo>
              );
            }
            return null;
          })}
        </div>
      ))}
    </div>
  );
};

export default Menu;
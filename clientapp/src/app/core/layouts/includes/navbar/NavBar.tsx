import './navBar.css';
import logo from '../../../../../assets/images/logo_.png';
import React, {useEffect, useRef} from 'react';
import appsettings from '../../../../common/config/appsettings';
import RouteTo from '../../../../_components/RouteTo';
import BlogAppRoutes from '../../../BlogAppRoutes';
import { useAppStore } from '../../../../common/services/appservices';
import AppRoutes from '../../../../../routes/AppRoutes';
import UserProfile from '../../../../_components/UserProfile';

const NavBar = () => {
  const [menuControl, setMenuControl] = React.useState(false);
  const {authService} = useAppStore();
  const [userProfileNavDisplay, setUserProfileNavDisplay] = React.useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const avatarRef = useRef<HTMLImageElement>(null);
 
  useEffect(() => {
    const handleClickOutside = (evt: MouseEvent) => {
      if (userProfileNavDisplay &&
          dropdownRef.current && 
          !dropdownRef.current.contains(evt.target as Node) && avatarRef.current && !avatarRef.current.contains(evt.target as Node)) {
            setUserProfileNavDisplay(false);
        }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [userProfileNavDisplay]);

  return (
    <>
    <nav className="w-full h-16 md:h-20 flex items-center justify-between relative bg-white border-b-1 shadow-md">
    <div className="flex items-center gap-4 text-2xl font-bold" id="btnMenu">
        <a href={BlogAppRoutes().public.home.parentRoute}><img src={logo} alt={appsettings.appName} title={appsettings.appName} srcSet="" style={{width:'200px', marginLeft:'50px'}} loading='lazy'/></a>
        <span style={{lineHeight: 'initial', color: 'rgb(8, 29, 86)', fontWeight: 'bolder', fontSize: '2em'}}><span
            style={{color:'rgb(236, 236, 236)'}}>DM</span>
        </span>
    </div>
    <div className='flex'>
        <ul id="menu_control" className={`menuOptions ${menuControl && 'menuShow'}`}>
            {authService.UserProfile().isSuccess && authService.UserProfile().result?.roles &&
            <li>
                <RouteTo to={AppRoutes().dashboard.home.parentRoute} className='rounded-xl bg-onaxPurple px-3 py-2'>Dashboard🚀</RouteTo>
            </li>}
            <li>              
              <UserProfile/>
            </li>
        </ul>
        {/* <section className='flex gap-4'>
            {authService.UserProfile().isSuccess && authService.UserProfile().result?.roles ? <>{authService.UserProfile().result?.roles.some(r=> ['admin'].includes(r)) ? <RouteTo to={AppRoutes().dashboard.home.parentRoute}>{authService.UserProfile().result?.firstName}</RouteTo> : null}</>: null}
        </section> */}
    </div>
    <div className="md:hidden">
        <div className="md:hidden">
            <div className="cursor-pointer text-4xl absolute" id="btnToggle" onClick={() => setMenuControl(prev => !prev)} style={{userSelect: 'none',right:'10px', top:'0px', zIndex: '10'}}>
                <span id="btnToggleDisplay">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" style={{width:'50px',height:'50px',color:'gray'}} viewBox="0 0 24 24" strokeWidth="1.9" stroke="currentColor" className="size-6"><path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" /></svg>
                </span>
            </div>
        </div>
    </div>
</nav>
    </>
  )
}

export default NavBar
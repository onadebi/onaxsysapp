import './navBar.css';
import logo from '../../../../../assets/images/logo_.png';
import React, {useEffect, useRef, useState } from 'react';
import appsettings from '../../../../common/config/appsettings';
import RouteTo from '../../../../_components/RouteTo';
import BlogAppRoutes from '../../../BlogAppRoutes';
import { useAppStore } from '../../../../common/services/appservices';
import { useNavigate } from 'react-router-dom';
import avatar from '../../../../../assets/images/avatar.png';
import logout from '../../../../../assets/images/logout.png';
import setting from '../../../../../assets/images/setting.png';
import FormModalControlled from '../../../../_components/FormModalControlled';
import { UserLoginResponseUpdateDTO } from '../../../../common/models/UserLoginResponse';

const NavBar = () => {
  const [menuControl, setMenuControl] = React.useState(false);
  const {authService} = useAppStore();
  const navigate = useNavigate();
  const [userProfileNavDisplay, setUserProfileNavDisplay] = React.useState(false);
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
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

  
  const LogOut = async () => {
    const confirmLogout = window.confirm('Are you sure you want to logout?');
    if(!confirmLogout){setUserProfileNavDisplay(false); return;}
    const objResp = await authService.logout();
    if(objResp){navigate(BlogAppRoutes().public.login.parentRoute);}
  }


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
            <li>              
              {
                authService.UserProfile().isSuccess 
                ? 
                <aside className='flex items-center gap-[1rem]'>
                    <RouteTo to={BlogAppRoutes().dashboard.home.parentRoute} className='rounded-xl bg-onaxPurple px-3 py-2'>Dashboard🚀</RouteTo>
                    <span className='cursor-pointer relative' title={`${authService.UserProfile().result?.firstName}`}>
                    <img src={avatar} alt='avatar' ref={avatarRef} className='rounded-full w-8 h-8' onClick={()=> setUserProfileNavDisplay(prev=> !prev)} />
                        <div ref={dropdownRef} className={`border bg-[white] absolute top-19 right-0 rounded-md mt-2 min-w-[250px] overflow-hidden ${!userProfileNavDisplay && 'hidden'}`} style={{whiteSpace:'nowrap'}} >
                            <ul className='userprofile_menu'>
                                <li className='flex items-center gap-2'>
                                    <img src={setting} alt='avatar' className='rounded-full w-4 h-4' />
                                    <span onClick={() =>{ setIsFormModalOpen(true); setUserProfileNavDisplay(false)}}>Manage account</span>
                                </li>
                                <li title={`logout`} onClick={LogOut}  className='flex items-center gap-2'>
                                    <img src={logout} alt='avatar' className='rounded-full w-4 h-4' />
                                    Sign out
                                </li>
                            </ul>
                        </div>
                    </span> 
                    <span>
                    <FormModalControlled id={`rgdtee-54444ye-365643-we56356`} table='userProfile' type='update' title={`Edit User profile`} data={{} as UserLoginResponseUpdateDTO}
                    open={isFormModalOpen}
                    onClose={() => setIsFormModalOpen(false)} />
                    </span>
                </aside>
                : <RouteTo to={BlogAppRoutes().public.login.parentRoute} className='rounded-xl bg-onaxPurple px-3 py-2'>Login🗝️</RouteTo>
              }
            </li>
        </ul>
        <section className='flex gap-4'>
            {authService.UserProfile().isSuccess && authService.UserProfile().result?.roles ? <>{authService.UserProfile().result?.roles.some(r=> ['admin'].includes(r)) ? <RouteTo to={BlogAppRoutes().dashboard.home.parentRoute}>{authService.UserProfile().result?.firstName}</RouteTo> : null}</>: null}
        </section>
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
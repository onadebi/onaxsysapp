import { useRef, useState, useEffect } from 'react';
import { useAppStore } from '../common/services/appservices';
import RouteTo from './RouteTo';
import avatar from '../../assets/images/avatar.png';
import logout from '../../assets/images/logout.png';
import setting from '../../assets/images/setting.png';
import AppRoutes from '../../routes/AppRoutes';
import { useNavigate } from 'react-router-dom';
import FormModalControlled from './FormModalControlled';
import { UserLoginResponseUpdateDTO } from '../common/models/UserLoginResponse';

const UserProfile = () => {
    const { authService } = useAppStore();
    const navigate = useNavigate();
    const [userProfileNavDisplay, setUserProfileNavDisplay] = useState(false);
    const [isFormModalOpen, setIsFormModalOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);
    const avatarRef = useRef<HTMLImageElement>(null);
    const [dropdownPosition, setDropdownPosition] = useState('right-0');

    const LogOut = async () => {
        const confirmLogout = window.confirm('Are you sure you want to logout?');
        if (!confirmLogout) { setUserProfileNavDisplay(false); return; }
        const objResp = await authService.logout();
        if (objResp) { navigate(AppRoutes().public.login.parentRoute); }
    };

    useEffect(() => {
        const handleResize = () => {
            if (avatarRef.current && dropdownRef.current) {
                const avatarRect = avatarRef.current.getBoundingClientRect();
                const dropdownRect = dropdownRef.current.getBoundingClientRect();
                const viewportWidth = window.innerWidth;

                if (avatarRect.right + dropdownRect.width > viewportWidth) {
                    setDropdownPosition('right-0');
                } else if (avatarRect.left - dropdownRect.width < 0) {
                    setDropdownPosition('left-0');
                } else {
                    setDropdownPosition('left-1/2 transform -translate-x-1/2');
                }
            }
        };

        handleResize();
        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, [userProfileNavDisplay]);

    return (
        authService.UserProfile().isSuccess
            ?
            <aside className='flex items-center gap-[1rem]'>
                <span className='cursor-pointer relative' title={`${authService.UserProfile().result?.firstName}`}>
                    <img src={authService.UserProfile().result?.picture ?? avatar} alt='avatar' ref={avatarRef} className='rounded-full w-8 h-8' onClick={() => setUserProfileNavDisplay(prev => !prev)} />
                    <div ref={dropdownRef} className={`border bg-[white] absolute top-19 ${dropdownPosition} rounded-md mt-2 min-w-[250px] overflow-hidden ${!userProfileNavDisplay && 'hidden'}`} style={{ whiteSpace: 'nowrap' }}>
                        <ul className='userprofile_menu'>
                            <li className='flex items-center gap-2'>
                                <img src={setting} alt='avatar' className='rounded-full w-4 h-4' />
                                <span onClick={() => { setIsFormModalOpen(true); setUserProfileNavDisplay(false) }}>Manage account</span>
                            </li>
                            <li title={`logout`} onClick={LogOut} className='flex items-center gap-2'>
                                <img src={logout} alt='avatar' className='rounded-full w-4 h-4' />
                                Sign out
                            </li>
                        </ul>
                    </div>
                </span>
                <span>
                    <FormModalControlled id={authService.UserProfile().result?.guid} table='userProfile' type='update' title={`Edit User profile`} data={{...authService.UserProfile().result} as UserLoginResponseUpdateDTO}
                        open={isFormModalOpen}
                        onClose={() => setIsFormModalOpen(false)} />
                </span>
            </aside>
            : <RouteTo to={AppRoutes().public.login.parentRoute} className='rounded-xl bg-onaxPurple px-3 py-2'>Login🗝️</RouteTo>
    );
};

export default UserProfile;
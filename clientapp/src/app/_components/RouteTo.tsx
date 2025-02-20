import React from 'react';
import { NavLink } from 'react-router-dom';

interface IProps{
    to: string;
    children: React.ReactNode;
    className?: string;
    title?: string
}

const RouteTo:React.FC<IProps> = ({to, children,className,title}) => {
  return (
    <>
        <NavLink to={to} className={`${className}`} onClick={() => window.scrollTo(0, 0)} title={title}>
            {children}
        </NavLink>
    </>
  )
}

export default RouteTo
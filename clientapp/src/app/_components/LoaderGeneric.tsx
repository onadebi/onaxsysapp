import React from 'react'

const LoaderGeneric: React.FC<{display: boolean, icon?: string,message?: string}> = ({display= false, icon='/assets/images/rocket-loader.gif', message='Loading'}) => {
  
    return (
      <div style={{ display:`${display ? 'block':'none'}`,zIndex:999 , background:'rgba(0,0,50,0.5)'}}
        className="fixed inset-0 flex items-center justify-center bg-[#ececec] bg-opacity-50 z-100"
      >
        <div className='flex items-center justify-center rounded-xl h-full'>
            <div className="text-center text-onaxLavendarDark font-semibold bg-white p-4 rounded-lg shadow-md flex items-center">
            <img src={icon} alt="loader" className="w-20 h-20 mx-auto" />
              {message ? message : <span>Loading...</span>}
            </div>
        </div>
      </div>
    );
  };

export default LoaderGeneric
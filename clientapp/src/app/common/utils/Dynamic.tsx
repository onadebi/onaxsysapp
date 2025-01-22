/* eslint-disable @typescript-eslint/no-explicit-any */
import React, {lazy, Suspense, ComponentType} from 'react';

interface DynamicImportProps{
    fallback: React.ReactNode;
}

const Dynamic = (importFunc: () => Promise<{default: ComponentType<any>}>, {fallback}:DynamicImportProps) => {
    const LazyComponent = lazy(importFunc);
    return (props: any)=> (
        <Suspense fallback={fallback}>
            <LazyComponent {...props} />
        </Suspense>
    );
}

export default Dynamic
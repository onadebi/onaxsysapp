import React, {lazy, Suspense, ComponentType} from 'react';

interface DynamicImportProps<T> {
    fallback: React.ReactNode;
    type?: T;
  }

const Dynamic =<T extends object> (importFunc: () => Promise<{default: ComponentType<T>}>, {fallback}:DynamicImportProps<T>) => {
    const LazyComponent = lazy(importFunc);
    return (props: T)=> (
        <Suspense fallback={fallback}>
            <LazyComponent {...(props as T)} />
        </Suspense>
    );
}

export default Dynamic
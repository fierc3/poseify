import { Navigate, Outlet } from 'react-router-dom';
import useClaims, {useLoginCheck } from '../../helpers/claims';

const AuthUserLayout = () => {
    const {isLoading } = useClaims()
    const [isLoggedIn] = useLoginCheck();

    if(!isLoggedIn() && !isLoading){
        return <Navigate to={"/"} replace />;
    }else{
        return <Outlet />
    }
};

export default AuthUserLayout;
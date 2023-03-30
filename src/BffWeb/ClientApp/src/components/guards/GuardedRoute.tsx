import React, { FC } from 'react';
import { Route, Redirect } from "react-router-dom";
import useClaims from '../../helpers/claims';

interface ParentCompProps {
    childComp?: React.ReactNode;
  }


const GuardedRoute: FC<ParentCompProps> = (props) => {
    const {childComp} = props
    const { data: claims, isLoading } = useClaims();
    let logoutUrl = claims?.find((claim: { type: string; }) => claim.type === 'bff:logout_url') 
    let nameDict = claims?.find((claim: { type: string; }) => claim.type === 'name') ||  claims?.find((claim: { type: string; }) => claim.type === 'sub');
    let username = nameDict?.value; 
  
    if(isLoading)
    return <div>Loading...</div>
  
  return ( 
    <Route render={() => {
          !username ?
              <Redirect to='/' /> :
              <childComp />;
      } } />
  )
  }

export default GuardedRoute;
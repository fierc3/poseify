import { TextField } from '@fluentui/react';
import { CompoundButton } from '@fluentui/react/lib/Button';
import { FC, useCallback, useState } from 'react';
import { getCurrentUserProfile, isReturnData } from '../../helpers/api';
import useClaims from '../../helpers/claims';

const Visitor: FC = () => {
  const { data: claims, isLoading } = useClaims();
  let logoutUrl = claims?.find((claim: { type: string; }) => claim.type === 'bff:logout_url') 
  let nameDict = claims?.find((claim: { type: string; }) => claim.type === 'name') ||  claims?.find((claim: { type: string; }) => claim.type === 'sub');
  let username = nameDict?.value; 

  if(isLoading)
  return <div>Loading...</div>

return ( 
  <div>
    {
      !username ? (
        <a 
          href="/bff/login?returnUrl=/"
        >
          Login
        </a>
      ) : (
        <div>
          <div>
            <div>
              <p>{`Hi, ${username}!`}</p>
              <a 
                href={logoutUrl?.value}
              >
                Logout
              </a>
            </div>
          </div>
        </div>
      )
    }
  </div>
)
}

export default Visitor;

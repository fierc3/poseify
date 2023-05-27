import axios from 'axios';
import { useCallback } from 'react';
import { useQuery } from 'react-query';

const claimsKeys = {
  claim: ['claims']
}

const config = {
  headers: {
    'X-CSRF': '1'
  }
}

const fetchClaims = async () =>
  axios.get('/bff/user', config)
    .then((res) => res.data);


function useClaims() {
  return useQuery(
    claimsKeys.claim,
    async () => fetchClaims(),
    {
      staleTime: Infinity,
      cacheTime: Infinity,
      retry: false,
      refetchInterval: 1000 * 15,
    }
  )
}


export const useLoginCheck =() => {
  const { data: claims } = useClaims();

  const isLoggedIn = useCallback(() => {
    let nameDict = claims?.find((claim: { type: string; }) => claim.type === 'name') ||  claims?.find((claim: { type: string; }) => claim.type === 'sub');
    let username = nameDict?.value; 

    return username !== undefined;
  },[claims])

  return [isLoggedIn]
}



export { useClaims as default }
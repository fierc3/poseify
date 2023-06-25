import axios from 'axios';
import { useQuery } from 'react-query';
import { IEstimation } from './api.types';
import useClaims from './claims';

const claimsKeys = {
  claim: ['estimations']
}

const config = {
  headers: {
    'X-CSRF': '1'
  }
}

const fetchData = async () =>
  axios.get('/api/GetUserEstimations', config)
  .then((res) => res.data as IEstimation[]);

function useEstimations() {
  return useQuery(
    claimsKeys.claim,
    async () => fetchData(),
    {
      staleTime: Infinity,
      cacheTime: Infinity,
      refetchInterval: 1000 * 15,
    }
  )
}

export { useEstimations as default }


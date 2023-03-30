import { TextField } from '@fluentui/react';
import { CompoundButton } from '@fluentui/react/lib/Button';
import { FC, useCallback, useState } from 'react';
import Navbar from '../../components/navbar/default-navbar';
import { getCurrentUserProfile, isReturnData } from '../../helpers/api';
import useClaims from '../../helpers/claims';

const Home: FC = () => {


return ( 
  <>
  This is home content
  </>
)
}

export default Home;

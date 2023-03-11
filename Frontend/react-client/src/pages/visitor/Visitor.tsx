import { TextField } from '@fluentui/react';
import { CompoundButton } from '@fluentui/react/lib/Button';
import { useCallback, useState } from 'react';
import { getCurrentUserProfile, isReturnData } from '../../helpers/api';

const Visitor = () => {
  const [userProfileId, setUserProfileId] = useState('');

  const onChangeProfileId = useCallback(
    (value: string | undefined) => {
        setUserProfileId(value || '');
    },
    [],
  );

  return (
    <div className="App">
      <p>Visitor</p>
      <CompoundButton primary secondaryText='Output user data in console' onClick={async() => {
        const data = await getCurrentUserProfile(userProfileId);
        if(isReturnData(data)){
          console.log("Succcess", data)
        }else{
          console.warn("WWWWWWWWWWWWWAAAAAAAAAAH")
        }
      }}>
      </CompoundButton>
      <TextField label="enter id pls" value={userProfileId} onChange={(_e, v) => onChangeProfileId(v)}></TextField>
    </div>
  );
}

export default Visitor;

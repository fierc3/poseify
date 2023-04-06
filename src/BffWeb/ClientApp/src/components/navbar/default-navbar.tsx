import { CommandBar, ICommandBarItemProps, Stack } from '@fluentui/react';
import useClaims, { useLoginCheck } from '../../helpers/claims';

const DefaultNavbar = (args: any) => {
  const [isLoggedIn] = useLoginCheck();
  const { data: claims } = useClaims();
  let logoutUrl = claims?.find((claim: { type: string; }) => claim.type === 'bff:logout_url') 
  let nameDict = claims?.find((claim: { type: string; }) => claim.type === 'name') ||  claims?.find((claim: { type: string; }) => claim.type === 'sub');
  let username = nameDict?.value; 

  const openUrl = (url: string) => {
    window.open(url, '_self', 'noreferrer');
  };

  const items: ICommandBarItemProps[] = [
    {
      key: 'homeItem',
      text: 'Home',
      cacheKey: 'myCacheKey', // changing this key will invalidate this item's cache
      iconProps: { iconName: 'Home' },
    }]

  const farItems: ICommandBarItemProps[] = []

  const userAction: ICommandBarItemProps[] = isLoggedIn() ? [
    {
      key: 'user',
      text: 'User Settings',
      // This needs an ariaLabel since it's icon-only
      ariaLabel: 'User Settings Page',
      iconOnly: true,
      iconProps: { iconName: 'Settings' },
      onClick: () => console.log("open settings")
    },
    {
      key: 'logout',
      text: 'Logout, ' + username,
      // This needs an ariaLabel since it's icon-only
      ariaLabel: 'Logout User',
      iconProps: { iconName: 'Logout' },
      onClick: () => openUrl(logoutUrl?.value ?? 'shit'),
    }] : [{
      key: 'login',
      text: 'Login',
      // This needs an ariaLabel since it's icon-only
      ariaLabel: 'Login User',
      iconProps: { iconName: 'Login' },
      onClick: () => openUrl('/bff/login?returnUrl=/'),
    }]

  return (
    <Stack style={{ height: 60 }} horizontal>
      <Stack.Item grow={1}>
        <Stack horizontal style={{ paddingTop: 10, paddingLeft: 10 }}>
          <h1>Poseify</h1>
        </Stack>
      </Stack.Item>
      <Stack.Item grow={10}>
        <CommandBar
          items={items}
          farItems={[...farItems, ...userAction]}
          ariaLabel="Inbox actions"
          primaryGroupAriaLabel="Main actions"
          farItemsGroupAriaLabel="User actions"
        />
      </Stack.Item>
    </Stack>
  );
}

export default DefaultNavbar;
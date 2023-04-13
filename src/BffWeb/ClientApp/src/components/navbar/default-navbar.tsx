import { CommandBar, IButtonProps, ICommandBarItemProps, Stack } from "@fluentui/react";
import { useNavigate } from "react-router-dom";
import useClaims, { useLoginCheck } from "../../helpers/claims";

const DefaultNavbar = () => {
  const [isLoggedIn] = useLoginCheck();
  const navigate = useNavigate();
  const { data: claims } = useClaims();
  let logoutUrl = claims?.find(
    (claim: { type: string }) => claim.type === "bff:logout_url"
  );
  let nameDict =
    claims?.find((claim: { type: string }) => claim.type === "name") ||
    claims?.find((claim: { type: string }) => claim.type === "sub");
  let username = nameDict?.value;

  const openUrl = (url: string) => {
    window.open(url, "_self", "noreferrer");
  };

  const overflowProps: IButtonProps = { ariaLabel: 'More' };

  const items: ICommandBarItemProps[] = [
    {
      key: "homeItem",
      text: "Home",
      cacheKey: "myCacheKey", // changing this key will invalidate this item's cache
      iconProps: { iconName: "Home" },
      onClick: () => navigate("/"),
    },
  ];

  const userActions: ICommandBarItemProps[] = isLoggedIn()
    ? [
        {
          key: "dashboardItem",
          text: "Dashboard",
          iconProps: { iconName: "AddHome" },
          onClick: () => navigate("/dashboard"),
        },
      ]
    : [];

  const farItems: ICommandBarItemProps[] = [];

  const userFarActions: ICommandBarItemProps[] = isLoggedIn()
    ? [
        {
          key: "user",
          text: "User Settings",
          // This needs an ariaLabel since it's icon-only
          ariaLabel: "User Settings Page",
          iconOnly: true,
          iconProps: { iconName: "Settings" },
          onClick: () => navigate('/Settings'),
        },
        {
          key: "logout",
          text: "Logout, " + username,
          // This needs an ariaLabel since it's icon-only
          ariaLabel: "Logout User",
          iconProps: { iconName: "Logout" },
          onClick: () => openUrl(logoutUrl?.value ?? "shit"),
        },
      ]
    : [
        {
          key: "login",
          text: "Login",
          // This needs an ariaLabel since it's icon-only
          ariaLabel: "Login User",
          iconProps: { iconName: "Login" },
          onClick: () => openUrl("/bff/login?returnUrl=/"),
        },
      ];

  return (
    <Stack style={{ height: 60 }} horizontal>
      <Stack.Item grow={1}>
        <Stack horizontal style={{ paddingTop: 10, paddingLeft: 10 }}>
          <h1>Poseify</h1>
        </Stack>
      </Stack.Item>
      <Stack.Item grow={10}>
        <CommandBar
          items={[...items, ...userActions]}
          farItems={[...farItems, ...userFarActions]}
          ariaLabel="Inbox actions"
          primaryGroupAriaLabel="Main actions"
          farItemsGroupAriaLabel="User actions"
          overflowButtonProps={overflowProps}
          overflowItems={[]}
        />
      </Stack.Item>
    </Stack>
  );
};

export default DefaultNavbar;

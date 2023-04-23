import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';
import LoginIcon from '@mui/icons-material/Login';
import useClaims, { useLoginCheck } from '../../helpers/claims';
import { useNavigate } from 'react-router-dom';
import { FC } from "react";

const DefaultNavbar: FC = () => {
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
  interface menuOptions {
    key: string,
    text: string,
    onClick: () => void
  }

  const pages: menuOptions[] = [
    {
      key: "homeItem",
      text: "Home",
      onClick: () => navigate("/"),
    },
  ];

  const userActions: menuOptions[] = isLoggedIn()
    ? [
      {
        key: "dashboardItem",
        text: "Dashboard",
        onClick: () => navigate("/dashboard"),
      },
    ]
    : [];

  const settings: menuOptions[] = isLoggedIn()
    ? [
      {
        key: "user",
        text: "User Settings",
        onClick: () => navigate('/Settings'),
      },
      {
        key: "logout",
        text: "Logout, " + username,
        onClick: () => openUrl(logoutUrl?.value ?? "shit"),
      },
    ]
    : [
      {
        key: "login",
        text: "Login",
        onClick: () => openUrl("/bff/login?returnUrl=/"),
      }
    ];

  const [anchorElNav, setAnchorElNav] = React.useState<null | HTMLElement>(null);
  const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(null);

  const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElNav(event.currentTarget);
  };
  const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleCloseNavMenu = () => {
    setAnchorElNav(null);
  };

  const handleCloseUserMenu = () => {
    setAnchorElUser(null);
  };

  return (
    <AppBar position="static">
      <Container maxWidth="xl">
        <Toolbar disableGutters>
          <Typography
            variant="h6"
            noWrap
            component="a"
            href="/"
            sx={{
              mr: 2,
              display: { xs: 'none', md: 'flex' },
              fontFamily: 'monospace',
              fontWeight: 700,
              letterSpacing: '.3rem',
              color: 'inherit',
              textDecoration: 'none',
            }}
          >
            POSEIFY
          </Typography>

          <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
            <IconButton
              size="large"
              aria-label="account of current user"
              aria-controls="menu-appbar"
              aria-haspopup="true"
              onClick={handleOpenNavMenu}
              color="inherit"
            >
              <MenuIcon />
            </IconButton>
            <Menu
              id="menu-appbar"
              anchorEl={anchorElNav}
              anchorOrigin={{
                vertical: 'bottom',
                horizontal: 'left',
              }}
              keepMounted
              transformOrigin={{
                vertical: 'top',
                horizontal: 'left',
              }}
              open={Boolean(anchorElNav)}
              onClose={handleCloseNavMenu}
              sx={{
                display: { xs: 'block', md: 'none' },
              }}
            >
              {[...pages, ...userActions].map((page) => (
                <MenuItem key={page.key} onClick={page.onClick}>
                  <Typography textAlign="center">{page.text}</Typography>
                </MenuItem>
              ))}
            </Menu>
          </Box>
          <Typography
            variant="h5"
            noWrap
            component="a"
            href=""
            sx={{
              mr: 2,
              display: { xs: 'flex', md: 'none' },
              flexGrow: 1,
              fontFamily: 'monospace',
              fontWeight: 700,
              letterSpacing: '.3rem',
              color: 'inherit',
              textDecoration: 'none',
            }}
          >
            POSEIFY
          </Typography>
          <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
            {[...pages, ...userActions].map((page) => (
              <Button
                key={page.key}
                onClick={page.onClick}
                sx={{ my: 2, color: 'white', display: 'block' }}
              >
                {page.text}
              </Button>
            ))}
          </Box>

          <Box sx={{ flexGrow: 0 }}>
            {isLoggedIn() ? (<>
              <Tooltip title="Open settings">
                <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                  <Avatar alt="User Profile Icon" src="https://media.tenor.com/D61XbAM9d9MAAAAd/doom-doomguy.gif" />
                </IconButton>
              </Tooltip>
              <Menu
                sx={{ mt: '45px' }}
                id="menu-appbar"
                anchorEl={anchorElUser}
                anchorOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                keepMounted
                transformOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                open={Boolean(anchorElUser)}
                onClose={handleCloseUserMenu}
              >
                {settings.map((setting) => (
                  <MenuItem key={setting.key} onClick={setting.onClick}>
                    <Typography textAlign="center">{setting.text}</Typography>
                  </MenuItem>
                ))}
              </Menu>
            </>) : (
              <IconButton onClick={() => openUrl("/bff/login?returnUrl=/")} sx={{ p: 0 }}>
                <LoginIcon color='secondary' />
              </IconButton>)}
          </Box>
        </Toolbar>
      </Container>
    </AppBar>
  );
}

export default DefaultNavbar;

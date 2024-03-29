import { Route } from "react-router-dom";
import Dashboard from "../pages/dashboard/dashboard";
import Home from "../pages/home/home";
import Settings from "../pages/settings/settings";

// Routes that can be accesssed without any claims
export const publicRoutes = <Route path="/" element={<Home />} />;
// Routes that can only be accessed when logged in
export const loggedInRoutes = (
  <>
    <Route path="/Dashboard" element={<Dashboard />} />
    <Route path="/Settings" element={<Settings />} />
  </>
);

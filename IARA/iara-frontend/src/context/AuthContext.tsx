import { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { User, UserRole, LoginRequest, RegisterRequest, AuthResponse } from '../types';
import { authAPI } from '../api/services';

interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isInspector: boolean;
  isProfessionalFisher: boolean;
  isRecreationalFisher: boolean;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  // Load user from localStorage on mount
  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    const storedUser = localStorage.getItem('user');

    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  const login = async (credentials: LoginRequest) => {
    try {
      const response = await authAPI.login(credentials);
      const authData: AuthResponse = response.data;

      const userData: User = {
        id: authData.userId,
        fullName: authData.fullName,
        email: authData.email,
        role: authData.role,
      };

      setToken(authData.token);
      setUser(userData);

      localStorage.setItem('token', authData.token);
      localStorage.setItem('user', JSON.stringify(userData));
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Login failed');
    }
  };

  const register = async (data: RegisterRequest) => {
    try {
      const response = await authAPI.register(data);
      const authData: AuthResponse = response.data;

      const userData: User = {
        id: authData.userId,
        fullName: authData.fullName,
        email: authData.email,
        role: authData.role,
      };

      setToken(authData.token);
      setUser(userData);

      localStorage.setItem('token', authData.token);
      localStorage.setItem('user', JSON.stringify(userData));
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Registration failed');
    }
  };

  const logout = () => {
    authAPI.logout().catch(() => {}); // Call backend but don't wait
    setUser(null);
    setToken(null);
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  };

  const value: AuthContextType = {
    user,
    token,
    login,
    register,
    logout,
    isAuthenticated: !!user && !!token,
    isAdmin: user?.role === UserRole.ADMIN,
    isInspector: user?.role === UserRole.INSPECTOR,
    isProfessionalFisher: user?.role === UserRole.PROFESSIONAL_FISHER,
    isRecreationalFisher: user?.role === UserRole.RECREATIONAL_FISHER,
    loading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
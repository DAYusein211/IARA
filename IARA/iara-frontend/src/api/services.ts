import api from './axios';
import * as Types from '../types';

// Auth API
export const authAPI = {
  login: (data: Types.LoginRequest) => 
    api.post<Types.AuthResponse>('/auth/login', data),
  
  register: (data: Types.RegisterRequest) => 
    api.post<Types.AuthResponse>('/auth/register', data),
  
  logout: () => 
    api.post('/auth/logout'),
};

// Ships API
export const shipsAPI = {
  getAll: () => 
    api.get<Types.Ship[]>('/ships'),
  
  getById: (id: number) => 
    api.get<Types.Ship>(`/ships/${id}`),
  
  getByOwner: (ownerId: number) => 
    api.get<Types.Ship[]>(`/ships/owner/${ownerId}`),
  
  create: (data: Types.CreateShipRequest) => 
    api.post<Types.Ship>('/ships', data),
  
  update: (id: number, data: Partial<Types.CreateShipRequest>) => 
    api.put<Types.Ship>(`/ships/${id}`, data),
  
  delete: (id: number) => 
    api.delete(`/ships/${id}`),
};

// Permits API
export const permitsAPI = {
  getAll: () => 
    api.get<Types.Permit[]>('/permits'),
  
  getById: (id: number) => 
    api.get<Types.Permit>(`/permits/${id}`),
  
  getByShip: (shipId: number) => 
    api.get<Types.Permit[]>(`/permits/ship/${shipId}`),
  
  getExpiring: (days: number = 30) => 
    api.get<Types.Permit[]>(`/permits/expiring?days=${days}`),
  
  create: (data: Types.CreatePermitRequest) => 
    api.post<Types.Permit>('/permits', data),
  
  update: (id: number, data: Partial<Types.CreatePermitRequest>) => 
    api.put<Types.Permit>(`/permits/${id}`, data),
  
  delete: (id: number) => 
    api.delete(`/permits/${id}`),
};

// Fishing Trips API
export const fishingTripsAPI = {
  getAll: () => 
    api.get<Types.FishingTrip[]>('/fishingtrips'),
  
  getById: (id: number) => 
    api.get<Types.FishingTrip>(`/fishingtrips/${id}`),
  
  getByShip: (shipId: number) => 
    api.get<Types.FishingTrip[]>(`/fishingtrips/ship/${shipId}`),
  
  getActive: () => 
    api.get<Types.FishingTrip[]>('/fishingtrips/active'),
  
  getCompleted: () => 
    api.get<Types.FishingTrip[]>('/fishingtrips/completed'),
  
  create: (data: Types.CreateFishingTripRequest) => 
    api.post<Types.FishingTrip>('/fishingtrips', data),
  
  update: (id: number, data: Partial<Types.CreateFishingTripRequest>) => 
    api.put<Types.FishingTrip>(`/fishingtrips/${id}`, data),
  
  complete: (id: number, fuelUsed?: number) => 
    api.post<Types.FishingTrip>(`/fishingtrips/${id}/complete?fuelUsed=${fuelUsed || ''}`),
  
  delete: (id: number) => 
    api.delete(`/fishingtrips/${id}`),
};

// Tickets API
export const ticketsAPI = {
  getAll: () => 
    api.get<Types.Ticket[]>('/tickets'),
  
  getById: (id: number) => 
    api.get<Types.Ticket>(`/tickets/${id}`),
  
  getByUser: (userId: number) => 
    api.get<Types.Ticket[]>(`/tickets/user/${userId}`),
  
  getActiveForUser: (userId: number) => 
    api.get<Types.Ticket>(`/tickets/user/${userId}/active`),
  
  buy: (data: Types.BuyTicketRequest) => 
    api.post<Types.Ticket>('/tickets/buy', data),
  
  delete: (id: number) => 
    api.delete(`/tickets/${id}`),
};

// Inspections API
export const inspectionsAPI = {
  getAll: () => 
    api.get<Types.Inspection[]>('/inspections'),
  
  getById: (id: number) => 
    api.get<Types.Inspection>(`/inspections/${id}`),
  
  getByInspector: (inspectorId: number) => 
    api.get<Types.Inspection[]>(`/inspections/inspector/${inspectorId}`),
  
  getByTarget: (targetType: number, targetId: number) => 
    api.get<Types.Inspection[]>(`/inspections/target?targetType=${targetType}&targetId=${targetId}`),
  
  getWithFines: () => 
    api.get<Types.Inspection[]>('/inspections/with-fines'),
  
  getUnpaidFines: () => 
    api.get<Types.Inspection[]>('/inspections/unpaid-fines'),
  
  create: (data: Types.CreateInspectionRequest) => 
    api.post<Types.Inspection>('/inspections', data),
  
  update: (id: number, data: Partial<Types.CreateInspectionRequest>) => 
    api.put<Types.Inspection>(`/inspections/${id}`, data),
  
  payFine: (id: number) => 
    api.post<Types.Inspection>(`/inspections/${id}/pay-fine`),
  
  delete: (id: number) => 
    api.delete(`/inspections/${id}`),
};

// Reports API
export const reportsAPI = {
  getExpiringPermits: (days: number = 30) => 
    api.get<Types.ExpiringPermitReport[]>(`/reports/expiring-permits?days=${days}`),
  
  getShipStatistics: (shipId: number, year: number) => 
    api.get<Types.ShipStatistics>(`/reports/ship-statistics/${shipId}?year=${year}`),
  
  getAllShipsStatistics: (year: number) => 
    api.get<Types.ShipStatistics[]>(`/reports/all-ships-statistics?year=${year}`),
  
  getCarbonFootprint: () => 
    api.get<Types.CarbonFootprint[]>('/reports/carbon-footprint'),
};
// Enums
export enum UserRole {
  ADMIN = 'ADMIN',
  INSPECTOR = 'INSPECTOR',
  PROFESSIONAL_FISHER = 'PROFESSIONAL_FISHER',
  RECREATIONAL_FISHER = 'RECREATIONAL_FISHER'
}

export enum FuelType {
  DIESEL = 'DIESEL',
  PETROL = 'PETROL',
  ELECTRIC = 'ELECTRIC',
  HYBRID = 'HYBRID'
}

export enum TicketType {
  DAILY = 'DAILY',
  WEEKLY = 'WEEKLY',
  MONTHLY = 'MONTHLY',
  YEARLY = 'YEARLY'
}

export enum InspectionResult {
  PASSED = 'PASSED',
  FAILED = 'FAILED',
  WARNING = 'WARNING'
}

export enum TargetType {
  SHIP = 'SHIP',
  FISHER = 'FISHER',
  FISHING_TRIP = 'FISHING_TRIP'
}

// Auth Types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  role: UserRole;
}

export interface AuthResponse {
  userId: number;
  fullName: string;
  email: string;
  role: UserRole;
  token: string;
}

export interface User {
  id: number;
  fullName: string;
  email: string;
  role: UserRole;
}

// Ship Types
export interface Ship {
  id: number;
  name: string;
  registrationNumber: string;
  ownerId: number;
  ownerName: string;
  enginePower: number;
  fuelType: FuelType;
  createdAt: string;
}

export interface CreateShipRequest {
  name: string;
  registrationNumber: string;
  ownerId: number;
  enginePower: number;
  fuelType: FuelType;
}

// Permit Types
export interface Permit {
  id: number;
  shipId: number;
  shipName: string;
  shipRegistrationNumber: string;
  validFrom: string;
  validTo: string;
  allowedGear: string;
  isExpired: boolean;
  daysUntilExpiry: number;
  createdAt: string;
}

export interface CreatePermitRequest {
  shipId: number;
  validFrom: string;
  validTo: string;
  allowedGear: string;
}

// Fishing Trip Types
export interface FishingTrip {
  id: number;
  shipId: number;
  shipName: string;
  shipRegistrationNumber: string;
  startTime: string;
  endTime: string | null;
  fuelUsed: number | null;
  isCompleted: boolean;
  durationHours: number | null;
  totalCatchKg: number;
  catches: Catch[];
  createdAt: string;
}

export interface Catch {
  id?: number;
  fishingTripId?: number;
  fishType: string;
  quantityKg: number;
  createdAt?: string;
}

export interface CreateFishingTripRequest {
  shipId: number;
  startTime: string;
  endTime?: string | null;
  fuelUsed?: number | null;
  catches: Catch[];
}

// Ticket Types
export interface Ticket {
  id: number;
  userId: number;
  userName: string;
  userEmail: string;
  validFrom: string;
  validTo: string;
  ticketType: TicketType;
  price: number;
  isActive: boolean;
  daysRemaining: number;
  createdAt: string;
}

export interface BuyTicketRequest {
  userId: number;
  ticketType: TicketType;
}

// Inspection Types
export interface Inspection {
  id: number;
  inspectorId: number;
  inspectorName: string;
  targetType: TargetType;
  targetId: number;
  targetDescription: string;
  inspectionDate: string;
  result: InspectionResult;
  notes: string;
  fine: Fine | null;
  createdAt: string;
}

export interface Fine {
  id: number;
  inspectionId: number;
  amount: number;
  reason: string;
  isPaid: boolean;
  createdAt: string;
}

export interface CreateInspectionRequest {
  inspectorId: number;
  targetType: TargetType;
  targetId: number;
  inspectionDate: string;
  result: InspectionResult;
  notes: string;
  fine?: {
    amount: number;
    reason: string;
  } | null;
}

// Report Types
export interface ExpiringPermitReport {
  permitId: number;
  shipId: number;
  shipName: string;
  shipRegistrationNumber: string;
  ownerName: string;
  ownerEmail: string;
  validFrom: string;
  validTo: string;
  daysUntilExpiry: number;
  allowedGear: string;
}

export interface ShipStatistics {
  shipId: number;
  shipName: string;
  registrationNumber: string;
  ownerName: string;
  fuelType: FuelType;
  enginePower: number;
  totalTrips: number;
  completedTrips: number;
  activeTrips: number;
  averageTripDurationHours: number | null;
  minTripDurationHours: number | null;
  maxTripDurationHours: number | null;
  yearlyCatchKg: number;
  totalCatchAllTimeKg: number;
  totalFuelUsed: number | null;
  averageFuelPerTrip: number | null;
  carbonFootprintRatio: number | null;
}

export interface CarbonFootprint {
  shipId: number;
  shipName: string;
  registrationNumber: string;
  fuelType: FuelType;
  enginePower: number;
  totalFuelUsed: number;
  totalCatchKg: number;
  fuelPerCatchRatio: number;
  efficiencyRating: string;
}
import { useState, useEffect } from 'react';
import { Navbar } from '../../components/Navbar';
import { useAuth } from '../../context/AuthContext';
import { Ship, Anchor, Fish, FileCheck } from 'lucide-react';
import { Edit, Trash2 } from 'lucide-react';
import { shipsAPI, fishingTripsAPI, permitsAPI } from '../../api/services';
import { Ship as ShipType, FishingTrip, Permit } from '../../types';
import { FishingTripForm } from './FishingTripForm';
export function FisherDashboard() {
    const handleDeleteTrip = async (id: number) => {
      if (!window.confirm('Are you sure you want to delete this fishing trip?')) return;
      try {
        await fishingTripsAPI.delete(id);
        await loadDashboardData();
      } catch (error: any) {
        alert(error.response?.data?.message || 'Failed to delete fishing trip');
      }
    };
  const { user } = useAuth();
  const [ships, setShips] = useState<ShipType[]>([]);
  const [activeTrips, setActiveTrips] = useState<FishingTrip[]>([]);
  const [completedTrips, setCompletedTrips] = useState<FishingTrip[]>([]);
  const [permits, setPermits] = useState<Permit[]>([]);
  const [loading, setLoading] = useState(true);
  const [showTripForm, setShowTripForm] = useState(false);
  const [editingTrip, setEditingTrip] = useState<FishingTrip | null>(null);

  useEffect(() => {
    loadDashboardData();
    // eslint-disable-next-line
  }, [user]);

  const loadDashboardData = async () => {
    if (!user) return;
    try {
      const [shipsRes, activeRes, completedRes] = await Promise.all([
        shipsAPI.getByOwner(user.id),
        fishingTripsAPI.getActive(),
        fishingTripsAPI.getCompleted(),
      ]);
      setShips(shipsRes.data);
      // Filter trips for user's ships
      const userShipIds = shipsRes.data.map(s => s.id);
      setActiveTrips(activeRes.data.filter(t => userShipIds.includes(t.shipId)));
      setCompletedTrips(completedRes.data.filter(t => userShipIds.includes(t.shipId)));
      // Load permits for user's ships
      if (shipsRes.data.length > 0) {
        const permitPromises = shipsRes.data.map(ship => permitsAPI.getByShip(ship.id));
        const permitResults = await Promise.all(permitPromises);
        const allPermits = permitResults.flatMap(res => res.data);
        setPermits(allPermits);
      }
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const stats = {
    totalShips: ships.length,
    activeTrips: activeTrips.length,
    totalCatch: activeTrips.reduce((sum, trip) => sum + (trip.totalCatchKg || 0), 0),
    activePermits: permits.filter(p => p.isExpired === false).length,
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Fisher Dashboard</h1>
          <p className="text-gray-600 mt-2">Manage your ships and fishing trips</p>
        </div>
        {loading ? (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
            <p className="text-gray-600 mt-4">Loading dashboard...</p>
          </div>
        ) : (
          <>
            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">My Ships</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.totalShips}</p>
                  </div>
                  <Ship className="h-8 w-8 text-blue-600" />
                </div>
              </div>
              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Active Trips</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.activeTrips}</p>
                  </div>
                  <Anchor className="h-8 w-8 text-green-600" />
                </div>
              </div>
              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Total Catch (kg)</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.totalCatch.toFixed(1)}</p>
                  </div>
                  <Fish className="h-8 w-8 text-purple-600" />
                </div>
              </div>
              <div className="bg-white rounded-lg shadow p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">Active Permits</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stats.activePermits}</p>
                  </div>
                  <FileCheck className="h-8 w-8 text-yellow-600" />
                </div>
              </div>
            </div>
            {/* Ships List + Log Trip */}
            <div className="bg-white rounded-lg shadow">
              <div className="p-6 border-b border-gray-200 flex items-center justify-between">
                <h2 className="text-xl font-semibold text-gray-900">My Ships</h2>
                <button
                  className="bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition"
                  onClick={() => setShowTripForm(true)}
                  disabled={ships.length === 0}
                >
                  + Log Fishing Trip
                </button>
              </div>
              <div className="p-6">
                {ships.length === 0 ? (
                  <p className="text-gray-600 text-center py-8">You don't have any registered ships yet.</p>
                ) : (
                  <div className="space-y-4">
                    {ships.map(ship => (
                      <div key={ship.id} className="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition">
                        <div className="flex items-center justify-between">
                          <div>
                            <h3 className="font-semibold text-gray-900">{ship.name}</h3>
                            <p className="text-sm text-gray-600">Registration: {ship.registrationNumber}</p>
                            <p className="text-sm text-gray-600">Engine: {ship.enginePower} HP • {ship.fuelType}</p>
                          </div>
                          <Ship className="h-8 w-8 text-blue-600" />
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
            <FishingTripForm
              isOpen={showTripForm}
              onClose={() => { setShowTripForm(false); setEditingTrip(null); }}
              onSuccess={loadDashboardData}
              ships={ships}
              initialTrip={editingTrip}
              isEdit={!!editingTrip}
            />
            {/* Active Trips */}
            {activeTrips.length > 0 && (
              <div className="bg-white rounded-lg shadow mt-6">
                <div className="p-6 border-b border-gray-200">
                  <h2 className="text-xl font-semibold text-gray-900">Active Fishing Trips</h2>
                </div>
                <div className="p-6">
                  <div className="space-y-4">
                    {activeTrips.map(trip => (
                      <div key={trip.id} className="border border-gray-200 rounded-lg p-4">
                        <div className="flex items-center justify-between">
                          <div>
                            <h3 className="font-semibold text-gray-900">{trip.shipName}</h3>
                            <p className="text-sm text-gray-600">Started: {new Date(trip.startTime).toLocaleString()}</p>
                            <p className="text-sm text-gray-600">Total Catch: {trip.totalCatchKg} kg</p>
                          </div>
                          <div className="text-right flex flex-col items-end gap-2">
                            <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800">
                              In Progress
                            </span>
                            <div className="flex gap-2 mt-2">
                              <button
                                className="text-blue-600 hover:text-blue-900"
                                onClick={() => { setEditingTrip(trip); setShowTripForm(true); }}
                                title="Edit Trip"
                              >
                                <Edit className="h-5 w-5 inline" />
                              </button>
                              <button
                                className="text-red-600 hover:text-red-900"
                                onClick={() => handleDeleteTrip(trip.id)}
                                title="Delete Trip"
                              >
                                <Trash2 className="h-5 w-5 inline" />
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            )}

            {/* Completed Trips */}
            {completedTrips.length > 0 && (
              <div className="bg-white rounded-lg shadow mt-6">
                <div className="p-6 border-b border-gray-200">
                  <h2 className="text-xl font-semibold text-gray-900">Completed Fishing Trips</h2>
                </div>
                <div className="p-6">
                  <div className="space-y-4">
                    {completedTrips.map(trip => (
                      <div key={trip.id} className="border border-gray-200 rounded-lg p-4">
                        <div className="flex items-center justify-between">
                          <div>
                            <h3 className="font-semibold text-gray-900">{trip.shipName}</h3>
                            <p className="text-sm text-gray-600">Started: {new Date(trip.startTime).toLocaleString()}</p>
                            <p className="text-sm text-gray-600">Ended: {trip.endTime ? new Date(trip.endTime).toLocaleString() : 'N/A'}</p>
                            <p className="text-sm text-gray-600">Total Catch: {trip.totalCatchKg} kg</p>
                          </div>
                          <div className="text-right flex flex-col items-end gap-2">
                            <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-gray-200 text-gray-800">
                              Completed
                            </span>
                            <div className="flex gap-2 mt-2">
                              <button
                                className="text-blue-600 hover:text-blue-900"
                                onClick={() => { setEditingTrip(trip); setShowTripForm(true); }}
                                title="Edit Trip"
                              >
                                <Edit className="h-5 w-5 inline" />
                              </button>
                              <button
                                className="text-red-600 hover:text-red-900"
                                onClick={() => handleDeleteTrip(trip.id)}
                                title="Delete Trip"
                              >
                                <Trash2 className="h-5 w-5 inline" />
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
}
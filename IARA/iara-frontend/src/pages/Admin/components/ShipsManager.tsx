import { useState, useEffect } from 'react';
import { Ship as ShipIcon, Plus, Edit, Trash2 } from 'lucide-react';
import { Modal } from '../../../components/shared/Modal';
import { Button } from '../../../components/shared/Button';
import { shipsAPI } from '../../../api/services';
import { Ship, FuelType } from '../../../types';

export const ShipsManager = () => {
  const [ships, setShips] = useState<Ship[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingShip, setEditingShip] = useState<Ship | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    registrationNumber: '',
    ownerId: 3, // Default to fisher user
    enginePower: 0,
    fuelType: FuelType.DIESEL,
  });

  useEffect(() => {
    loadShips();
  }, []);

  const loadShips = async () => {
    try {
      const res = await shipsAPI.getAll();
      setShips(res.data);
    } catch (error) {
      console.error('Error loading ships:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingShip) {
        await shipsAPI.update(editingShip.id, formData);
      } else {
        await shipsAPI.create(formData);
      }
      await loadShips();
      handleCloseModal();
    } catch (error: any) {
      alert(error.response?.data?.message || 'Failed to save ship');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this ship?')) return;
    try {
      await shipsAPI.delete(id);
      await loadShips();
    } catch (error: any) {
      alert(error.response?.data?.message || 'Failed to delete ship');
    }
  };

  const handleOpenModal = (ship?: Ship) => {
    if (ship) {
      setEditingShip(ship);
      setFormData({
        name: ship.name,
        registrationNumber: ship.registrationNumber,
        ownerId: ship.ownerId,
        enginePower: ship.enginePower,
        fuelType: ship.fuelType,
      });
    } else {
      setEditingShip(null);
      setFormData({
        name: '',
        registrationNumber: '',
        ownerId: 3,
        enginePower: 0,
        fuelType: FuelType.DIESEL,
      });
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setEditingShip(null);
  };

  if (loading) {
    return <div className="text-center py-8">Loading ships...</div>;
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-gray-900">Ships Management</h2>
        <Button onClick={() => handleOpenModal()}>
          <Plus className="h-5 w-5 mr-2 inline" />
          Add Ship
        </Button>
      </div>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Name</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Registration</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Owner</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Engine Power</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Fuel Type</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {ships.map((ship) => (
              <tr key={ship.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="flex items-center">
                    <ShipIcon className="h-5 w-5 text-blue-600 mr-2" />
                    <span className="font-medium text-gray-900">{ship.name}</span>
                  </div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{ship.registrationNumber}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{ship.ownerName}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{ship.enginePower} HP</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{ship.fuelType}</td>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <button
                    onClick={() => handleOpenModal(ship)}
                    className="text-blue-600 hover:text-blue-900 mr-3"
                  >
                    <Edit className="h-5 w-5 inline" />
                  </button>
                  <button
                    onClick={() => handleDelete(ship.id)}
                    className="text-red-600 hover:text-red-900"
                  >
                    <Trash2 className="h-5 w-5 inline" />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <Modal
        isOpen={showModal}
        onClose={handleCloseModal}
        title={editingShip ? 'Edit Ship' : 'Add New Ship'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Ship Name</label>
            <input
              type="text"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Registration Number</label>
            <input
              type="text"
              value={formData.registrationNumber}
              onChange={(e) => setFormData({ ...formData, registrationNumber: e.target.value })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
              disabled={!!editingShip}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Owner ID</label>
            <input
              type="number"
              value={formData.ownerId}
              onChange={(e) => setFormData({ ...formData, ownerId: parseInt(e.target.value) })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
              min={0}
             
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Engine Power (HP)</label>
            <input
              type="number"
              value={formData.enginePower}
              onChange={(e) => setFormData({ ...formData, enginePower: parseFloat(e.target.value) })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
              
              min={0}
               max={10000}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Fuel Type</label>
            <select
              value={formData.fuelType}
              onChange={(e) => setFormData({ ...formData, fuelType: e.target.value as FuelType })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            >
              {Object.values(FuelType).map(type => (
                <option key={type} value={type}>{type}</option>
              ))}
            </select>
          </div>

          <div className="flex space-x-3 pt-4">
            <Button type="submit" className="flex-1">
              {editingShip ? 'Update Ship' : 'Create Ship'}
            </Button>
            <Button type="button" variant="secondary" onClick={handleCloseModal} className="flex-1">
              Cancel
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
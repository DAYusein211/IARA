import { useState, useEffect } from 'react';
import {  Plus, Edit, Trash2, AlertTriangle } from 'lucide-react';
import { Modal } from '../../../components/shared/Modal';
import { Button } from '../../../components/shared/Button';
import { permitsAPI, shipsAPI } from '../../../api/services';
import { Permit, Ship } from '../../../types';

export const PermitsManager = () => {
  const [permits, setPermits] = useState<Permit[]>([]);
  const [ships, setShips] = useState<Ship[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingPermit, setEditingPermit] = useState<Permit | null>(null);
  const [formData, setFormData] = useState({
    shipId: 0,
    validFrom: '',
    validTo: '',
    allowedGear: '',
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [permitsRes, shipsRes] = await Promise.all([
        permitsAPI.getAll(),
        shipsAPI.getAll(),
      ]);
      setPermits(permitsRes.data);
      setShips(shipsRes.data);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingPermit) {
        await permitsAPI.update(editingPermit.id, formData);
      } else {
        await permitsAPI.create(formData);
      }
      await loadData();
      handleCloseModal();
    } catch (error: any) {
      alert(error.response?.data?.message || 'Failed to save permit');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this permit?')) return;
    try {
      await permitsAPI.delete(id);
      await loadData();
    } catch (error: any) {
      alert(error.response?.data?.message || 'Failed to delete permit');
    }
  };

  const handleOpenModal = (permit?: Permit) => {
    if (permit) {
      setEditingPermit(permit);
      setFormData({
        shipId: permit.shipId,
        validFrom: permit.validFrom.split('T')[0],
        validTo: permit.validTo.split('T')[0],
        allowedGear: permit.allowedGear,
      });
    } else {
      setEditingPermit(null);
      setFormData({
        shipId: ships[0]?.id || 0,
        validFrom: '',
        validTo: '',
        allowedGear: '',
      });
    }
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setEditingPermit(null);
  };

  if (loading) {
    return <div className="text-center py-8">Loading permits...</div>;
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold text-gray-900">Permits Management</h2>
        <Button onClick={() => handleOpenModal()}>
          <Plus className="h-5 w-5 mr-2 inline" />
          Issue Permit
        </Button>
      </div>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ship</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Valid From</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Valid To</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Allowed Gear</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {permits.map((permit) => (
              <tr key={permit.id} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="font-medium text-gray-900">{permit.shipName}</div>
                  <div className="text-sm text-gray-500">{permit.shipRegistrationNumber}</div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                  {new Date(permit.validFrom).toLocaleDateString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                  {new Date(permit.validTo).toLocaleDateString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {permit.isExpired ? (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                      <AlertTriangle className="h-3 w-3 mr-1" />
                      Expired
                    </span>
                  ) : permit.daysUntilExpiry < 30 ? (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800">
                      <AlertTriangle className="h-3 w-3 mr-1" />
                      {permit.daysUntilExpiry} days
                    </span>
                  ) : (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                      Active
                    </span>
                  )}
                </td>
                <td className="px-6 py-4 text-sm text-gray-600">{permit.allowedGear}</td>
                <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <button
                    onClick={() => handleOpenModal(permit)}
                    className="text-blue-600 hover:text-blue-900 mr-3"
                  >
                    <Edit className="h-5 w-5 inline" />
                  </button>
                  <button
                    onClick={() => handleDelete(permit.id)}
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
        title={editingPermit ? 'Edit Permit' : 'Issue New Permit'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Ship</label>
            <select
              value={formData.shipId}
              onChange={(e) => setFormData({ ...formData, shipId: parseInt(e.target.value) })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
            >
              <option value="">Select a ship</option>
              {ships.map(ship => (
                <option key={ship.id} value={ship.id}>
                  {ship.name} ({ship.registrationNumber})
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Valid From</label>
            <input
              type="date"
              value={formData.validFrom}
              onChange={(e) => setFormData({ ...formData, validFrom: e.target.value })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Valid To</label>
            <input
              type="date"
              value={formData.validTo}
              onChange={(e) => setFormData({ ...formData, validTo: e.target.value })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Allowed Gear</label>
            <textarea
              value={formData.allowedGear}
              onChange={(e) => setFormData({ ...formData, allowedGear: e.target.value })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              rows={3}
              placeholder="e.g., Trawl, Gillnet, Longline"
              required
            />
          </div>

          <div className="flex space-x-3 pt-4">
            <Button type="submit" className="flex-1">
              {editingPermit ? 'Update Permit' : 'Issue Permit'}
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
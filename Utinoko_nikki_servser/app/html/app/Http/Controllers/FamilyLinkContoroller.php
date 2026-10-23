<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class FamilyLinkContoroller extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');
        $elderUserId = $request->query('elder_user_id');
        $familyUserId = $request->query('family_user_id');

        $query = DB::table('family_links')
            ->select('id', 'elder_user_id', 'family_user_id', 'relationship');

        if ($id !== null && $id !== '') {
            $query->where('id', $id);
        }

        if ($elderUserId !== null && $elderUserId !== '') {
            $query->where('elder_user_id', $elderUserId);
        }

        if ($familyUserId !== null && $familyUserId !== '') {
            $query->where('family_user_id', $familyUserId);
        }

        return response()->json($query->get());
    }
}

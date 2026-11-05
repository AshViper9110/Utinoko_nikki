<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class FamilyLinkController extends Controller
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

    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'elder_user_id' => 'required|integer|exists:elder_users,id',
            'family_user_id' => 'required|integer|exists:family_users,id',
            'relationship' => 'required|string|max:255',
        ]);

        // 重複チェック
        $exists = DB::table('family_links')
            ->where('elder_user_id', $validated['elder_user_id'])
            ->where('family_user_id', $validated['family_user_id'])
            ->exists();

        if ($exists) {
            return response()->json([
                'message' => '既に家族として登録されています'
            ], 409);
        }

        // 登録
        $id = DB::table('family_links')->insertGetId([
            'elder_user_id' => $validated['elder_user_id'],
            'family_user_id' => $validated['family_user_id'],
            'relationship' => $validated['relationship'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録したデータを取得
        $familyLink = DB::table('family_links')
            ->where('id', $id)
            ->first();

        return response()->json($familyLink, 201);
    }
}
